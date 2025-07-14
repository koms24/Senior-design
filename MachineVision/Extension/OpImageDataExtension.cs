using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.DataType;
using SeniorDesignFall2024.MachineVision.ImageOp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.Extension
{
    public static class OpImageDataExtension
    {
        public static StatisticsValues GetStatsForMatColorSpaceChannel(this ImageData d, Mat m, string colorSpace, int channel)
        {
            Dictionary<string, Dictionary<int, StatisticsValues>> lookup = d.GetOpDataLastForKeyStrict<MeanStdDevOp, Dictionary<string, Dictionary<int, StatisticsValues>>>(MeanStdDevOp.OpDataKey_Stats, m);
            return lookup[colorSpace][channel];
        }
    }
}
