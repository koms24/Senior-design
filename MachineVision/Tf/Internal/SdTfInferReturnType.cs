using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.Tf.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.Tf.Internal
{
    [StructLayout(LayoutKind.Sequential, Size = 64)]
    public struct _cs_return_type
    {
        public IntPtr ptr_detection_boxes_mat;
        public IntPtr ptr_detection_classes_mat;
        public IntPtr ptr_detection_masks_mat;
        public IntPtr ptr_detection_scores_mat;
        public IntPtr ptr_image_info_mat;
        public Int32 __reserved_1;
        public Int32 detection_cnt;
        public IntPtr __next_ptr;
        public Int32 __reserved_2;
        public Int32 __last_size;
        public _cs_return_type()
        {
            ptr_detection_boxes_mat = IntPtr.Zero;
            ptr_detection_classes_mat = IntPtr.Zero;
            ptr_detection_masks_mat = IntPtr.Zero;
            ptr_detection_scores_mat = IntPtr.Zero;
            ptr_image_info_mat = IntPtr.Zero;
            __reserved_1 = 0;
            detection_cnt = 0;
            __next_ptr = IntPtr.Zero;
            __reserved_2 = 0;
            __last_size = 0;
        }
    }

    public class SdTfInferReturnType
    {
        private _cs_return_type _data;
        public Mat detection_boxes;
        public Mat detection_classes;
        public Mat detection_masks;
        public Mat detection_scores;
        public Mat image_info;
        public Int32 detection_cnt => _data.detection_cnt;
        public SdTfInferReturnType(IntPtr in_ptr, int idx, out IntPtr out_ptr)
        {
            SdTfNativeMethods.sdtf_TfController_getInferReturnStruct(in_ptr, idx, out _data);
            out_ptr = _data.__next_ptr != IntPtr.Zero ? _data.__next_ptr : IntPtr.Zero;
            detection_boxes = Mat.FromNativePointer(_data.ptr_detection_boxes_mat);
            detection_classes = Mat.FromNativePointer(_data.ptr_detection_classes_mat);
            detection_masks = Mat.FromNativePointer(_data.ptr_detection_masks_mat);
            detection_scores = Mat.FromNativePointer(_data.ptr_detection_scores_mat);
            image_info = Mat.FromNativePointer(_data.ptr_image_info_mat);
        }
    }

    public class SdTfInferReturnArray : IDisposable
    {
        private int _size;
        private IntPtr _ptr;
        private SdTfInferReturnType[] _cs_data;
        private bool disposedValue;

        public SdTfInferReturnArray(IntPtr d_ptr, int size)
        {
            _size = size;
            _ptr = d_ptr;
            IEnumerable<SdTfInferReturnType> _cs = Enumerable.Empty<SdTfInferReturnType>();
            IntPtr p = d_ptr;
            try {
                for (int i = 0; i < _size; i++) {
                    if (p == IntPtr.Zero) throw new Exception();
                    _cs = _cs.Append(new SdTfInferReturnType(p, i, out p));
                }
            } catch (Exception e) {
                freePtr();
                throw new Exception($"InitFailed:{nameof(SdTfInferReturnArray)}", e);
            }
            freePtr();
            _cs_data = _cs.ToArray();
        }

        public SdTfInferReturnType[] Data => _cs_data;

        private void freePtr() { 
            if(_ptr != IntPtr.Zero)
                SdTfNativeMethods.sdtf_TfController_deleteInferReturnStructArray(out _ptr);
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
        ~SdTfInferReturnArray()
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
