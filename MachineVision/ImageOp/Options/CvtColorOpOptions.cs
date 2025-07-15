using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.ImageOp.Options
{
    public class CvtColorOpOptions
    {
        public OpenCvSharp.ColorConversionCodes ColorConversionCode = OpenCvSharp.ColorConversionCodes.BGR2HSV_FULL;
        public string SrcColorSpace { get; set; } = "BGR";
        public string DstColorSpace { get; set; } = "HSV";
        public bool EmitResult { get; set; } = true;
    }
}
