using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.Abstract;
using SeniorDesignFall2024.MachineVision.DataType;
using SeniorDesignFall2024.MachineVision.Extension;
using SeniorDesignFall2024.MachineVision.ImageOp.Options;
using SeniorDesignFall2024.MachineVision.Tf.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.ImageOp
{
    public class CvtColorOp : AbstractOneToOneOp, IDisposable
    {
        public static string OpDataKey_Result = $"OpDataKey::{nameof(CvtColorOp)}::Result";
        private bool disposedValue;

        private CvtColorOpOptions options;
        public CvtColorOpOptions Opts => options;

        public CvtColorOp(IServiceProvider? serviceProvider, CvtColorOpOptions? opts = null) : base(serviceProvider)
        {
            if(opts != null)
                options = opts;
            else
                options = serviceProvider?.GetService<CvtColorOpOptions>() ?? new CvtColorOpOptions();
        }

        public override ImageData Run(ImageData input)
        {
            IEnumerable<Mat> results = input.Mats.Select(img =>
            {
                Mat result = input.Tracker.T(img.CvtColor(options.ColorConversionCode));
                input.AddOpDataForKeyLast<MeanStdDevOp, Mat>(
                    img,
                    OpDataKey_Result,
                    result
                    );
                return result;
            });
            if (options.EmitResult)
                input.Result(results, null);
            return input;
        }

        public IEnumerable<Mat> GetColorSpace(ImageData input)
        {
            var mats = input.Mats;
            try
            {
                var r = mats.Select(m => input.GetOpDataLastForKeyStrict<CvtColorOp, Mat>(OpDataKey_Result, m));
                return r;
            }
            catch (Exception ex)
            {
            }
            Run(input);
            return mats.Select(m => input.GetOpDataLastForKeyStrict<CvtColorOp, Mat>(OpDataKey_Result, m));
        }

        private void freeUnmanaged()
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
                freeUnmanaged();
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~CvtColorOp()
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
