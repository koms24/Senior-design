using Microsoft.AspNetCore.Mvc;
using SeniorDesignFall2024.ImagingDriver.Services;
using OpenCvSharp;
using SeniorDesignFall2024.Server.Services.OpenHab;
using Microsoft.Extensions.Options;
using SeniorDesignFall2024.Server.Shared.Options;
using SeniorDesignFall2024.ImagingDriver.Options;

namespace SeniorDesignFall2024.Server.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagingController : ControllerBase
    {
        private bool _enabled = true;
        private bool _cameraEnabled = false;
        private StreamOptions _streamOptions;
        private VideoStreamControllerService? videoService;
        private StillCaptureControllerService? stillService;
        private FileStreamerService? fileStreamerService;
        public ImagingController(
            IOptions<StartupConfigOptions> opts,
            IOptions<StreamOptions> streamOpts,
            VideoStreamControllerService? vidService = null,
            StillCaptureControllerService? stlService = null,
            FileStreamerService? fileStreamerService = null
            ) {
            _enabled = opts.Value.EnableImaging;
            _cameraEnabled = opts.Value.EnableCamera;
            _streamOptions = streamOpts.Value;
            this.videoService = vidService;
            this.stillService = stlService;
            this.fileStreamerService = fileStreamerService;
        }

        private bool checkEnabled()
        {
            if (!_enabled)
                throw new Exception("Imaging Feature Disabled, Check appsettings file");
            return _enabled;
        }

        private bool checkCameraEnabled()
        {
            if (!_cameraEnabled)
                throw new Exception("Camera Feature Disabled, Check appsettings file");
            return _enabled;
        }

        [HttpGet("StartStream")]
        public IActionResult StartStream()
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                if (videoService == null)
                    throw new Exception("VideoStreamControllerService not found");
                videoService?.StartStream();
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("StopStream")]
        public IActionResult StopStream()
        {
            Exception? ex = null;
            try
            {
                checkEnabled();
                videoService.StopStream();
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return ex == null ? Ok() : Problem(ex.ToString());
        }

        [HttpGet("Jpeg")]
        public async Task<IActionResult> GetJpeg(CancellationToken cancellationToken)
        {
            Exception? ex = null;
            //FileStreamResult? fsr = null;
            FileContentResult? fcr = null;
            try
            {
                checkEnabled();
                checkCameraEnabled();
                Mat img = stillService.CaptureStillImage();
                byte[] buf;
                Cv2.ImEncode(".jpg", img, out buf);
                fcr = new(buf, "image/jpeg");
                //var p = new System.IO.Pipelines.Pipe();
                //fsr = new FileStreamResult(p.Reader.AsStream(), "image/jpeg") {
                //    FileDownloadName = "still_capture.jpg"
                //};
                //await fileStreamerService.QueueFileStream((Stream stream) => img.WriteToStream(stream, ".jpg"), p.Writer.AsStream(), cancellationToken);
            }
            catch (Exception _ex)
            {
                ex = _ex;
            }
            return (fcr != null && ex == null) ? fcr : Problem((ex ?? new Exception("Unknown Server Error")).ToString());
        }
    }
}
