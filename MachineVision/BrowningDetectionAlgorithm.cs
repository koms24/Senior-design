using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace MachineVision
{
    public class BrowningDetectionAlgorithm
    {
        public static void DetectingBrown(string imgPath, string maskPath)
        {
            // Normalized dimensions for algotrithm
            int newWidth = 500;
            int newHeight = 400;

            // Threshold Values for Plant Health
            int criticallyDamagedEdge = 40;
            int criticallyDamaged = 30;
            int brownThresholdEdge = 800;
            int brownThreshold = 720;

            // Intake Image to process
            Mat img = Cv2.ImRead(imgPath);
            Mat mask = Cv2.ImRead(maskPath);

            // Create and resized normalized images
            Mat resizedImg = new Mat();
            Mat resizedMask = new Mat();
            Cv2.Resize(img,resizedImg, new Size(newWidth, newHeight));
            Cv2.Resize(mask, resizedMask, new Size(newWidth, newHeight));

            // Generate Masked Image
            Mat maskedImg = new Mat();
            Cv2.BitwiseAnd(resizedImg, resizedMask, maskedImg);

            // Create Gradient and Outside of Leaf
            Mat gradient = new Mat();
            Cv2.MorphologyEx(resizedMask, gradient, MorphTypes.Gradient, Cv2.GetStructuringElement(MorphShapes.Rect, new Size(6, 6)));
            Mat OutsideOfLeaf = new Mat();
            Cv2.BitwiseAnd(resizedImg, gradient, OutsideOfLeaf);

            // Generate Inverse Gradient and Inside of Leaf 
            Mat inverseGradient = new Mat();
            Cv2.BitwiseNot(gradient, inverseGradient);
            Cv2.BitwiseAnd(inverseGradient, resizedMask, inverseGradient);
            Mat InsideOfLeaf = new Mat();
            Cv2.BitwiseAnd(resizedImg, inverseGradient, InsideOfLeaf);

            
            //Display Image, Masks, and Bitwise Operations for Testing
            Cv2.ImShow("Regular Image", resizedImg);
            Cv2.WaitKey(0);
            Cv2.ImShow("Just Mask", resizedMask);
            Cv2.WaitKey(0);
            Cv2.ImShow("Result Image", maskedImg);
            Cv2.WaitKey(0);
            Cv2.ImShow("Gradient Mask", gradient);
            Cv2.WaitKey(0);
            Cv2.ImShow("Inverse Gradient Mask", inverseGradient);
            Cv2.WaitKey(0);
            Cv2.ImShow("Edge of Leaf", OutsideOfLeaf);
            Cv2.WaitKey(0);
            Cv2.ImShow("Inside of Leaf", InsideOfLeaf);
            Cv2.WaitKey(0);
          

            // Find Total Non-zero pizels in edge and inside of leaf for later calculations

            //Create Edge 2 Gray
            Mat edge2Gray = new Mat();
            Cv2.CvtColor(OutsideOfLeaf, edge2Gray, ColorConversionCodes.BGR2GRAY);
            
            // Create Edge Threshold
            Mat ThresholdEdge = new Mat();
            Cv2.Threshold(edge2Gray, ThresholdEdge, 1, 255, ThresholdTypes.Binary);

            // Calculate and Report total pixels in edges
            int TotalEdgePixels = Cv2.CountNonZero(ThresholdEdge);
            Console.WriteLine(TotalEdgePixels);

            //Create Inside 2 Gray
            Mat inside2Gray = new Mat();
            Cv2.CvtColor(InsideOfLeaf, inside2Gray, ColorConversionCodes.BGR2GRAY);

            // Create Edge Threshold
            Mat ThresholdInside = new Mat();
            Cv2.Threshold(inside2Gray, ThresholdInside, 1, 255, ThresholdTypes.Binary);

            // Calculate and Report total pixels in edges
            int TotalInsidePixels = Cv2.CountNonZero(ThresholdInside);
            Console.WriteLine(TotalInsidePixels);

            // Calculate Ratio of Edge to Inside pixels
            double ratioEdgeOverInside = (double)TotalEdgePixels / TotalInsidePixels;

            //Setup Browning bins 
            int[] brownRange = { 5, 30 };
            int startingBin = brownRange[0];
            int endingBin = brownRange[1];

            //IMG
            // Convert Image to HSV, isolate Hue Channel, and apply Histogram to it
            Mat img2HSV = new Mat();
            Cv2.CvtColor(maskedImg, img2HSV, ColorConversionCodes.BGR2HSV);
            
            // Isolate Hue Channel
            Mat hueChannel = new Mat(); // !!!
            Cv2.ExtractChannel(img2HSV, hueChannel, 0);

            // Generate Histogram
            Mat hueHistogram = new Mat();
            Cv2.CalcHist(new Mat[] { hueChannel }, new int[] { 0 }, null, hueHistogram, 1, new int[] { 180 }, new Rangef[] { new Rangef(0, 180) });

            // EDGE
            // Generate Edge Hue Channel and Histogram
            Mat img2HSVEdge = new Mat();
            Cv2.CvtColor(OutsideOfLeaf, img2HSVEdge, ColorConversionCodes.BGR2HSV);

            // Isolate Hue Channel
            Mat hueChannelEdge = new Mat(); // !!!
            Cv2.ExtractChannel(img2HSVEdge, hueChannelEdge, 0);

            // Generate Histogram
            Mat hueHistogramEdge = new Mat();
            Cv2.CalcHist(new Mat[] { hueChannelEdge }, new int[] { 0 }, null, hueHistogramEdge, 1, new int[] { 180 }, new Rangef[] { new Rangef(0, 180) });

            // Calculate total Brown Pixels in the Edge
            double totalBrownPixelsInEdge = (double)hueHistogramEdge.RowRange(startingBin, endingBin).Sum(); //!!!
            Console.WriteLine(totalBrownPixelsInEdge);
            double percentBrownEdge = (totalBrownPixelsInEdge / TotalEdgePixels) * 100;
            Console.WriteLine(percentBrownEdge);

            //INSIDE
            // Generate Inside Hue Channel and Histogram
            Mat img2HSVInside = new Mat();
            Cv2.CvtColor(InsideOfLeaf, img2HSVInside, ColorConversionCodes.BGR2HSV);

            // Isolate Hue Channel
            Mat hueChannelInside = new Mat(); // !!!
            Cv2.ExtractChannel(img2HSVInside, hueChannelInside, 0);

            // Generate Histogram
            Mat hueHistogramInside = new Mat();
            Cv2.CalcHist(new Mat[] { hueChannelInside }, new int[] { 0 }, null, hueHistogramInside, 1, new int[] { 180 }, new Rangef[] { new Rangef(0, 180) });

            // Calculate total Brown Pixels in the Leaf
            double totalBrownPixelsInside = (double)hueHistogramInside.RowRange(startingBin, endingBin).Sum(); //!!!
            Console.WriteLine(totalBrownPixelsInside);
            double percentBrownInside = (totalBrownPixelsInside / TotalInsidePixels) * 100;
            Console.WriteLine(percentBrownInside);


            /*
            // Run normalized edge Algorithm
            int noiseRange = 1;
            for (int i = 0; i < noiseRange; i++)
            {
                hueHistogramEdge[i] = 0;
            }
            Mat HueHistorgramEdgeNormalized = new Mat();
            Cv2.Normalize(hueHistogramEdge, HueHistorgramEdgeNormalized, 0, 255, NormTypes.MinMax);
            */


            // Create Scaled Histogram by subracting inside histogram from edge histogram
            Mat ScaledHueHistogram = hueHistogramEdge - hueHistogramInside;

            // Create Binary mask for Hue values 5-30
            Mat maskBrown = new Mat();
            Cv2.InRange(hueChannel, new Scalar(brownRange[0]), new Scalar (brownRange[1]), maskBrown);

            //Apply Mask to edge images
            Mat resultEdge = new Mat();
            Cv2.BitwiseAnd(OutsideOfLeaf, OutsideOfLeaf, resultEdge, maskBrown);

            // Resize to new dimensions
            Mat resizedResultEdge = new Mat();
            Cv2.Resize(resultEdge, resizedResultEdge, new Size(newWidth,newHeight));

            //Apply Mask to inside images
            Mat resultInside = new Mat();
            Cv2.BitwiseAnd(InsideOfLeaf, InsideOfLeaf, resultInside, maskBrown);

            // Resize to new dimensions
            Mat resizedResultInside = new Mat();
            Cv2.Resize(resultInside, resizedResultInside, new Size(newWidth, newHeight));

            // Show Browns on Edge and Inside
            Cv2.ImShow("Browns of Leaf Edges Image", resizedResultEdge);
            Cv2.WaitKey(0);
            Cv2.ImShow("Browns of Leaf Inside Image", resizedResultInside);
            Cv2.WaitKey(0);

            // Redo histogram with just the brown range we want
            Mat hueHistAdjusted = hueHistogram.RowRange(brownRange[0], brownRange[1]);
            Mat hueHistEdgeAdjusted = hueHistogramEdge.RowRange(brownRange[0], brownRange[1]);
            Mat hueHistInsideAdjusted = hueHistogramInside.RowRange(brownRange[0], brownRange[1]+1);

            // Add up the amount of brown detected
            double brownsDetected = (double)hueHistAdjusted.Sum();
            double brownsDetectedEdge = (double)hueHistEdgeAdjusted.Sum();
            double brownsDetectedInside = (double)hueHistInsideAdjusted.Sum();

            // Prints out status of Browning on leaf using threshold values from top
            //EDGE
            if (brownsDetectedEdge > brownThresholdEdge && percentBrownEdge > criticallyDamagedEdge)
            {
                Console.WriteLine("Your plant has a very unhealthy amount of browning on the edges. Immediate action required.");
            }
            else if (brownsDetectedEdge > brownThresholdEdge && percentBrownEdge < criticallyDamagedEdge)
            {
                Console.WriteLine("Your plant has a slightly unhealthy amount of browning on the edges.");
            }
            else
            {
                Console.WriteLine("No unhealthy browning detected in your plant.");
            }

            //LEAF
            if (brownsDetectedInside > brownThreshold && percentBrownInside > criticallyDamaged)
            {
                Console.WriteLine("Your plant has a very unhealthy amount of browning on the inside. Immediate action required.");
            }
            else if (brownsDetectedInside > brownThreshold && percentBrownInside < criticallyDamaged)
            {
                Console.WriteLine("Your plant has a slightly unhealthy amount of browning on the inside.");
            }
            else
            {
                Console.WriteLine("No unhealthy browning detected on the inside.");
            }
        }
    }
}
