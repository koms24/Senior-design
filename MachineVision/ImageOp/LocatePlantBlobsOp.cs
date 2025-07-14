using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.Abstract;
using SeniorDesignFall2024.MachineVision.DataType;
using SeniorDesignFall2024.MachineVision.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.ImageOp
{
    public class LocatePlantBlobsOp : AbstractOneToOneOp, IDisposable
    {
        public static string OpDataKey_KeyPoints = $"OpDataKey::{nameof(LocatePlantBlobsOp)}::KeyPoints";
        private bool disposedValue;
        private Mat kernel;
        private Mat blurKernel;
        private Scalar hsvRangeMinVal = new Scalar(25, 25, 25);
        private Scalar hsvRangeMaxVal = new Scalar(82, 255, 255);
        private SimpleBlobDetector blob_detector = SimpleBlobDetector.Create(new SimpleBlobDetector.Params()
        {
            MinThreshold = 0,
            MaxThreshold = 254,
            FilterByArea = true,
            MinArea = 100 * 400,
            MaxArea = 10000 * 400,
            FilterByCircularity = false,
            FilterByConvexity = false,
            FilterByInertia = false
        });

        public override ImageData Run(ImageData input)
        {
            foreach(Mat in_img in input.Mats)
            {
                using (Mat mask = new Mat())
                {
                    using (Mat img = new Mat())
                    {
                        Cv2.CvtColor(in_img, img, ColorConversionCodes.BGR2HSV);
                        Cv2.InRange(img, hsvRangeMinVal, hsvRangeMaxVal, mask);
                    }
                    Cv2.BitwiseNot(mask, mask);
                    Cv2.MorphologyEx(mask, mask, MorphTypes.Open, kernel);
                    Cv2.MorphologyEx(mask, mask, MorphTypes.Close, kernel);
                    Cv2.GaussianBlur(mask, mask, blurKernel.Size(), 0);
                    blob_detector.Empty();
                    KeyPoint[] keypoints = blob_detector.Detect(mask);
                    input.AddOpDataForKeyLast<LocatePlantBlobsOp, KeyPoint[]>(in_img, OpDataKey_KeyPoints, keypoints);
                }
            }
            return input;
        }

        public LocatePlantBlobsOp(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
            kernel = new Mat(9, 9, MatType.CV_8U);
            blurKernel = new Mat(35, 35, MatType.CV_8U);
            kernel.SetTo(1);
        }

        private void freeMatVars()
        {
            kernel.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                freeMatVars();
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~LocatePlantBlobsOp()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
