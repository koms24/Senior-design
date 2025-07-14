using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace SeniorDesignFall2024.ImagingDriver.SDCam
{
    public class SDCamController : IDisposable
    {
        protected IntPtr ptr = IntPtr.Zero;
        protected Mat? frame = null;
        private bool disposedValue;

        public SDCamController() {
            SDCam.Internal.NativeMethods.sdc_SDCamController_new(out ptr);
        }

        public Mat GetMatStillFromCamera()
        {
            IntPtr rptr = IntPtr.Zero;
            SDCam.Internal.NativeMethods.sdc_SDCamController_getMatStillFromCamera(ptr, out rptr);
            return (frame = Mat.FromNativePointer(rptr));
        }

        public void Test()
        {
            Mat m = GetMatStillFromCamera();
            Mat m2 = new Mat(m.Size(), m.Type());
            Size s = new Size();
            Cv2.Resize(m, m2, s, 0.25, 0.25, InterpolationFlags.Area);
            Cv2.ImShow("Test", m2);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(frame != null)
                        frame.Dispose();
                    // TODO: dispose managed state (managed objects)
                }
                if(ptr != IntPtr.Zero && disposing)
                    SDCam.Internal.NativeMethods.sdc_SDCamController_delete(ptr);
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SDCamController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
