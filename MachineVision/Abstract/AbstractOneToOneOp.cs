using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.DataType;
using SeniorDesignFall2024.MachineVision.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.Abstract
{
    public abstract class AbstractOneToOneOp : IImageOp
    {
        protected IServiceProvider? _serviceProvider;
        public AbstractOneToOneOp(IServiceProvider? serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public abstract ImageData Run(ImageData input);

        public virtual ImageData Run(Mat input, ResourcesTracker? t)
        {
            return Run(new[] { input }, t);
        }

        public virtual ImageData Run(IEnumerable<Mat> inputs, ResourcesTracker? t)
        {
            return Run(new ImageData(inputs, t));
        }
    }
}
