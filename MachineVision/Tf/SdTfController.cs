using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.Extension;
using SeniorDesignFall2024.MachineVision.Tf.Interface;
using SeniorDesignFall2024.MachineVision.Tf.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.Tf
{
    public class SdTfController : ISdTfController, IDisposable
    {
        private IntPtr _ptr = IntPtr.Zero;
        private bool disposedValue;
        private int _height;
        private int _width;
        private int _channels;
        private string _model_dir;

        public nint Ptr { get { return _ptr; } }

        public SdTfController(int height, int width, int channels, string model_dir = "") {
            _height = height;
            _width = width;
            _channels = channels;
            _model_dir = model_dir;
            SdTfNativeMethods.sdtf_TfController_new(out _ptr, height, width, channels, model_dir);
        }

        public SdTfInferReturnArray RunInferenceOnImage(Mat img)
        {
            if (img.Dims != 2 || img.Height != _height || img.Width != _width)
                throw new ArgumentOutOfRangeException($"Input image ('{nameof(img)}' Dims: {img.DebugDimsToString()}) doesn't match input dims of input [ {_height} x {_width} ] Channels=3");
            IntPtr _ptr_result = IntPtr.Zero;
            int batchSize = 1;
            int r = 0;
            try
            {
                r = SdTfNativeMethods.sdtf_TfController_runInferenceOnImage(_ptr, img.CvPtr, out _ptr_result);
            } catch (Exception ex) {
                if (_ptr_result != IntPtr.Zero)
                    SdTfNativeMethods.sdtf_TfController_deleteInferReturnStructArray(out _ptr_result);
                throw new Exception($"Native Method Failed: {nameof(SdTfNativeMethods.sdtf_TfController_runInferenceOnImage)}", ex);
            }
            var result = new SdTfInferReturnArray(_ptr_result, batchSize);
            //Thread.Sleep(100);
            return result;
        }

        public SdTfInferReturnArray RunInferenceOnBatch(Mat img)
        {
            if (img.Dims != 3)
                throw new ArgumentOutOfRangeException($"Input image ('{nameof(img)}': Not a batch matrix expected dims 3 img was {img.Dims}");
            IntPtr _ptr_result = IntPtr.Zero;
            int batchSize = img.Size(0);
            int r = 0;
            try
            {
                r = SdTfNativeMethods.sdtf_TfController_runAsBatch(_ptr, img.CvPtr, out _ptr_result);
            }
            catch (Exception ex)
            {
                if (_ptr_result != IntPtr.Zero)
                    SdTfNativeMethods.sdtf_TfController_deleteInferReturnStructArray(out _ptr_result);
                throw new Exception($"Native Method Failed: {nameof(SdTfNativeMethods.sdtf_TfController_runAsBatch)}", ex);
            }
            var result = new SdTfInferReturnArray(_ptr_result, batchSize);
            //Thread.Sleep(100);
            return result;
        }

        private void freePtr()
        {
            if(_ptr != IntPtr.Zero)
                SdTfNativeMethods.sdtf_TfController_delete(out _ptr);
            _ptr = IntPtr.Zero;
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
                freePtr();
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~SdTfController()
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
