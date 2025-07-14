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
    public abstract class AbstractOneToManyOp : IImageOpAry
    {
        protected IServiceProvider? _serviceProvider;
        public AbstractOneToManyOp(IServiceProvider? serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual IEnumerable<ImageData> Run(ImageData input)
        {
            return SingleRun(input);
        }

        public virtual IEnumerable<ImageData> Run(Mat input, ResourcesTracker? t)
        {
            return Run(new[] {input}, t);
        }

        public virtual IEnumerable<ImageData> Run(IEnumerable<ImageData> input)
        {
            return input.SelectMany(SingleRun);
        }

        public virtual IEnumerable<ImageData> Run(IEnumerable<Mat> inputs, ResourcesTracker? t)
        {
            return Run(new ImageData(inputs, t));
        }

        public abstract IEnumerable<ImageData> SingleRun(ImageData input);
    }
}
