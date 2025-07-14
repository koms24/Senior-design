using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.Abstract;
using SeniorDesignFall2024.MachineVision.DataType;
using SeniorDesignFall2024.MachineVision.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.ImageOp
{
    public class RoiBackMap
    {
        public Mat SourceMat { get; set; }
        public Mat RoiMat { get; set; }
        public Rect SourceRoiRect { get; set; }
    }
    public class ExtractPlantBlobsOp : AbstractOneToOneOp, IDisposable
    {
        public static string OpDataKey_RoiBackMaps = $"OpDataKey::{nameof(LocatePlantBlobsOp)}::KeyPoints";
        private bool disposedValue;

        public ExtractPlantBlobsOp(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
        }

        public ImageData Run(ImageData input, out RoiBackMap[] roiMap)
        {
            if (!input.HasOpData<LocatePlantBlobsOp>())
            {
                var locatePlantBlobOp = _serviceProvider?.GetService<LocatePlantBlobsOp>() ?? new LocatePlantBlobsOp(_serviceProvider);
                locatePlantBlobOp.Run(input);
            }
            var t = input.Tracker;
            IEnumerable<KeyValuePair<Mat, IEnumerable<Rect>>> bboxes = Enumerable.Empty<KeyValuePair<Mat, IEnumerable<Rect>>>();
            foreach (Mat in_img in input.Mats)
            {
                KeyPoint[] keypoints = input.GetOpDataForKeyString<LocatePlantBlobsOp, KeyPoint[]>(in_img, LocatePlantBlobsOp.OpDataKey_KeyPoints);
                bboxes = bboxes.Append(new KeyValuePair<Mat, IEnumerable<Rect>>(in_img, keypoints.Select(k => k.ScaleToTileWindow())));
            }
            var dat = bboxes.SelectMany(kp =>
            {
                return kp.Value.Select(r => {
                    Mat roi_mat = t.T(new Mat(kp.Key, r));
                    return new RoiBackMap()
                    {
                        SourceMat = kp.Key,
                        RoiMat = roi_mat,
                        SourceRoiRect = r
                    };
                });
            }).ToArray();
            input.Result(dat.Select(o => o.RoiMat));
            input.AddOpDataForKeyLast<ExtractPlantBlobsOp, IEnumerable<RoiBackMap>>(dat.Select(o => o.RoiMat), OpDataKey_RoiBackMaps, dat);
            roiMap = dat;
            return input;
        }

        public override ImageData Run(ImageData input)
        {
            return Run(input, out _);
        }

        private void freeMatVars()
        {
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
        ~ExtractPlantBlobsOp()
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
