using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.ImagingDriver.SDCam.Internal
{
    public static class NativeMethods
    {
        public const string DllExtern = "./libsd_cam_native.so";

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ulong sdc_SDCamController_sizeof();

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdc_SDCamController_new(out IntPtr returnValue);

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdc_SDCamController_getMatStillFromCamera(IntPtr self, out IntPtr returnValue);

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdc_SDCamController_delete(IntPtr mat);
    }
}
