using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.ImagingDriver.Options
{
    public class StreamOptions
    {
        public const string SectionName = "StreamConfig";
        public bool UseRpiCamInput { get; set; } = true;
        public string StreamInput { get; set; } = String.Empty;
        public int OutputWidth { get; set; } = 1536;
        public int OutputHeight { get; set; } = 864;
        public int OutputFramerate { get; set; } = 24;
    }
}
