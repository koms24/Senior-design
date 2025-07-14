using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.Tf.Internal
{
    public static class SdTfNativeMethods
    {
        public const string DllExtern = "libsd_tf_wrapper";

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ulong sdtf_TfController_sizeof();

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern int sdtf_TfController_new(out IntPtr returnValue, int height, int width, int channels, string model = "");

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdtf_TfController_delete(out IntPtr mat);

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdtf_TfController_initInferReturnStruct(IntPtr self, int batchSize, out IntPtr returnValueStructAry);

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdtf_TfController_freeInferReturnStruct(IntPtr returnValueStructAry, int batchSize = 0);

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdtf_TfController_deleteInferReturnStructArray(out IntPtr returnValueStructAry, int batchSize = 0);

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdtf_TfController_runInferOnBatchWithBboxConvert(IntPtr self, IntPtr mat, int height, int width, out IntPtr returnValueStructAry);

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdtf_TfController_runInferenceOnImage(IntPtr self, IntPtr mat, out IntPtr returnValueStructAry);

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdtf_TfController_runAsBatch(IntPtr self, IntPtr mat, out IntPtr returnValueStructAry);

        [Pure, DllImport(DllExtern, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int sdtf_TfController_getInferReturnStruct(IntPtr r_ptr, int idx, out _cs_return_type o_dat);
    }
}