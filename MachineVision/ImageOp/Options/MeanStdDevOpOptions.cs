using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.ImageOp.Options
{
    public class MeanStdDevOpOptions
    {
        public string ColorSpace { get; set; } = "HSV";
        public int[] Channels {  get; set; } = new int[] { 2 };
    }
}
