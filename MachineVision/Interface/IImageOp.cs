using OpenCvSharp;
using SeniorDesignFall2024.MachineVision.DataType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.MachineVision.Interface
{
    public interface IBaseImageOp
    {
    }

    public interface IImageOp : IBaseImageOp
    {
        ImageData Run(ImageData input);
        ImageData Run(Mat input, ResourcesTracker? t);
    }

    public interface IImageOpAry : IBaseImageOp
    {
        IEnumerable<ImageData> Run(ImageData input);
        IEnumerable<ImageData> Run(Mat input, ResourcesTracker? t);
        IEnumerable<ImageData> Run(IEnumerable<ImageData> input);
        IEnumerable<ImageData> Run(IEnumerable<Mat> input, ResourcesTracker? t);
        IEnumerable<ImageData> SingleRun(ImageData input);
    }
}
