using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.Abstract;
using SeniorDesignFall2024.MachineVision.DataType;
using SeniorDesignFall2024.MachineVision.ImageOp.Options;
using SeniorDesignFall2024.MachineVision.Tf.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.ImageOp
{
    public class StatisticsValues {
        public double Mean { get; set; }
        public double StdDev { get; set; }
        //public double Min {  get; set; }
        //public double Max { get; set; }
        //public double Mode { get; set; }
    }
    public class MeanStdDevOp : AbstractOneToOneOp, IDisposable
    {
        public static string OpDataKey_Stats = $"OpDataKey::{nameof(MeanStdDevOp)}::Stats";
        private bool disposedValue;

        private MeanStdDevOpOptions options;
        private CvtColorOp colorOp;

        public MeanStdDevOp(IServiceProvider? serviceProvider, MeanStdDevOpOptions? opts = null) : base(serviceProvider)
        {
            if(opts != null)
                options = opts;
            else
                options = serviceProvider?.GetService<MeanStdDevOpOptions>() ?? new MeanStdDevOpOptions();
            CvtColorOp? cOps = serviceProvider?.GetServices<CvtColorOp>().FirstOrDefault(o => o.Opts.EmitResult == false && o.Opts.DstColorSpace == options.ColorSpace);
            colorOp = cOps ?? new CvtColorOp(serviceProvider, new CvtColorOpOptions() { DstColorSpace = "HSV", EmitResult = false });
        }

        public override ImageData Run(ImageData input)
        {
            foreach(var img in colorOp.GetColorSpace(input)) {
                Mat[] split = img.Split();
                //switch(options.ColorSpace) {
                //    case "HSV":
                //    default:
                //        split = img.CvtColor(ColorConversionCodes.BGR2HSV_FULL).Split();
                //        break;
                //}
                HashSet<int> idxs = options.Channels.ToHashSet();
                StatisticsValues[] stats = new StatisticsValues[idxs.Count];
                foreach(var chIdx in options.Channels)
                    stats[chIdx] = CalcChannelStats(split[chIdx]);
                input.AddOpDataForKeyLast<MeanStdDevOp, Dictionary<string, Dictionary<int, StatisticsValues>>>(
                    img,
                    OpDataKey_Stats,
                    new Dictionary<string, Dictionary<int, StatisticsValues>>()
                    {
                        {
                        options.ColorSpace,
                        stats
                        .Select((v, i) => new KeyValuePair<int, StatisticsValues>(i, v)).Where(kp => idxs.Contains(kp.Key))
                        .ToDictionary()
                        }
                    }
                    );
            }
            return input;
        }

        public static StatisticsValues CalcChannelStats(Mat channel, Mat? mask = null)
        {
            Mat mean = new Mat();
            Mat std = new Mat();
            if(mask != null)
                Cv2.MeanStdDev(channel, mean, std, mask);
            else
                Cv2.MeanStdDev(channel, mean, std);
            return new StatisticsValues()
            {
                Mean = mean.At<double>(0),
                StdDev = std.At<double>(0)
            };
        }

        public static StatisticsValues[] CalcChannelsStats(Mat channels, Mat? mask = null)
        {
            Mat mean = new Mat();
            Mat std = new Mat();
            if (mask != null)
                Cv2.MeanStdDev(channels, mean, std, mask);
            else
                Cv2.MeanStdDev(channels, mean, std);
            double[] _mean;
            double[] _std;
            mean.GetArray<double>(out _mean);
            std.GetArray<double>(out _std);
            return _mean.Zip(_std, (m,s)=> new StatisticsValues()
            {
                Mean = m,
                StdDev = s
            }).ToArray();
        }

        private void freeUnmanaged()
        {

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
        ~MeanStdDevOp()
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
