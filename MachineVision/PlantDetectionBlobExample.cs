using OpenCvSharp;

namespace MachineVision
{
    public class PlantDetectionBlobExample
    {
        public static void Run(string path_to_input_image)
        {
            Mat kernel = new Mat(3, 3, MatType.CV_8U, new Scalar(0));                   //python line: 4
            kernel.SetTo(new Scalar(1));

            Mat img = Cv2.ImRead(path_to_input_image);                                        //python line: 5

            Mat img_gray = new Mat();                                                   //python line: 6
            Cv2.CvtColor(img, img_gray, ColorConversionCodes.BGR2GRAY);

            Mat img1 = new Mat();                                                       //python line: 8
            img.CopyTo(img1);

            Cv2.CvtColor(img1, img1, ColorConversionCodes.BGR2HSV);                     //python line: 9

            Mat mask = new Mat();                                                       //python line: 10
            Cv2.InRange(img1, new Scalar(30, 25, 25), new Scalar(82, 255, 255), mask);

            Cv2.BitwiseNot(mask, mask);                                                 //python line: 11

            Cv2.MorphologyEx(mask, mask, MorphTypes.Open, kernel);                      //python line: 12

            Cv2.MorphologyEx(mask, mask, MorphTypes.Close, kernel);                     //python line: 13

            Cv2.GaussianBlur(mask, mask, kernel.Size(), 0);                             //python line: 14

            Cv2.ImShow("Green Mask", mask);                                             //python line: 15

            Cv2.WaitKey(0);                                                             //python line: 16

            SimpleBlobDetector.Params @params = new SimpleBlobDetector.Params           //python lines: 19 to 27
            {
                MinThreshold = 0,
                MaxThreshold = 254,
                FilterByArea = true,
                MinArea = 50,
                MaxArea = 30000,
                FilterByCircularity = false,
                FilterByConvexity = false,
                FilterByInertia = false
            };

            SimpleBlobDetector detector = SimpleBlobDetector.Create(@params);           //python line: 28

            detector.Empty();                                                           //python line: 30

            int windowSize = 3;                                                         //python line: 32

            int windowConstant = 5;                                                     //python line: 33

            var keypoints = detector.Detect(mask);                                      //python line: 35

            Mat im_with_keypoints = new Mat();                                          //python line: 37
            Cv2.DrawKeypoints(img, keypoints, im_with_keypoints, new Scalar(0, 0, 255), DrawMatchesFlags.DrawRichKeypoints);

            Cv2.ImShow("Keypoints", im_with_keypoints);                                 //python line: 39
            Cv2.WaitKey(0);                                                             //python line: 40
            Cv2.DestroyAllWindows();                                                    //python line: 41
        }
    }
}
