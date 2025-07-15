using OpenCvSharp;
using OpenCvSharp.Dnn;
using SeniorDesignFall2024.MachineVision.Abstract;
using SeniorDesignFall2024.MachineVision.DataType;
using SeniorDesignFall2024.MachineVision.Extension;
using SeniorDesignFall2024.MachineVision.Tf;
using SeniorDesignFall2024.MachineVision.Tf.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.ImageOp
{
    public class LeafInferMaskRcnnOp : AbstractOneToOneOp, IDisposable
    {
        public static string OpDataKey_InferBatchResult = $"OpDataKey::{nameof(LeafInferMaskRcnnOp)}::InferBatchResult";
        private bool disposedValue;

        private SdTfController tf;

        public LeafInferMaskRcnnOp(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
            tf = new(256, 256, 3, "../../../../MachineVision/Tf/External/infer_sv_model_v3/");
        }

        public override ImageData Run(ImageData input)
        {
            Mat batch = input.Tracker.T(input.Mats.ToBatchMat());
            input.Result<LeafInferMaskRcnnOp>(new Mat[] { batch }, null, true);
            var result = tf.RunInferenceOnBatch(batch);
            input.AddOpDataForKeyLast<LeafInferMaskRcnnOp, SdTfInferReturnArray>(batch, OpDataKey_InferBatchResult, result);
            return input;
        }

        protected void freeUnmanaged()
        {
            tf.Dispose();
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
        ~LeafInferMaskRcnnOp()
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
