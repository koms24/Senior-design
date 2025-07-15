using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using SeniorDesignFall2024.ImagingDriver.SDCam;

namespace SeniorDesignFall2024.ImagingDriver.Services
{
    public class StillCaptureControllerService
    {
        private VideoStreamControllerService streamController;
        private SDCamController camController;
        public StillCaptureControllerService(
            VideoStreamControllerService _streamController,
            SDCamController _camController
            ) {
            streamController = _streamController;
            camController = _camController;
        }

        public Mat CaptureStillImage()
        {
            bool run = streamController.IsStreamRunning;
            if (run)
                streamController.StopStream();
            Mat img = camController.GetMatStillFromCamera();
            if(run)
                streamController.StartStream();
            return img;
        }
    }
}
