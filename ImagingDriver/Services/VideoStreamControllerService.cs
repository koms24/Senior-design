

using Microsoft.Extensions.Options;
using SeniorDesignFall2024.ImagingDriver.Options;
using System.Diagnostics;

namespace SeniorDesignFall2024.ImagingDriver.Services
{
    public class VideoStreamControllerService
    {
        private readonly ProcessStartInfo _rpicamVidInfo;
        private readonly ProcessStartInfo _ffmpegInfo;
        private Process? _rpicamVid = null;
        private Process? _ffmpeg = null;

        private HlsOptions options;
        private StreamOptions _streamOptions;
        private bool _isStreamRunning = false;

        private readonly VideoStreamPipe _pipe = new();
        private Task? _pipeTask = null;
        public VideoStreamControllerService(IOptions<HlsOptions> opts,
            IOptions<StreamOptions> streamOpts)
        {
            options = opts.Value;
            _streamOptions = streamOpts.Value;
            StreamOptions o = streamOpts.Value;
            string sdir = Path.GetFullPath(options.StreamFolderPath);
            _rpicamVidInfo = new ProcessStartInfo();
            _rpicamVidInfo.FileName = "rpicam-vid";
            _rpicamVidInfo.Arguments = $"-t 0 -n --width {o.OutputWidth} --height {o.OutputHeight} --framerate {o.OutputFramerate} --codec yuv420 --libav-format rawvideo -o -";
            _rpicamVidInfo.WorkingDirectory = sdir;
            _rpicamVidInfo.UseShellExecute = false;
            _rpicamVidInfo.RedirectStandardOutput = true;
            string ffargs = "-y ";
            if (o.UseRpiCamInput)
                ffargs += $"-f rawvideo -pix_fmt yuv420p -s:v {o.OutputWidth}x{o.OutputHeight} -re -fflags +genpts -i - ";
            else
                ffargs += $"-f dshow -pixel_format yuyv422 -video_size {o.OutputWidth}x{o.OutputHeight} -framerate {o.OutputFramerate} -fflags +genpts -i video=\"{o.StreamInput}\" ";
            //encode config
            ffargs += $"-c:v libx264 -g:v {o.OutputFramerate} -keyint_min:v {o.OutputFramerate} -sc_threshold:v 0 -preset ultrafast -tune zerolatency ";
            //streaming config
            ffargs += "-use_timeline 0 -streaming 1 -window_size 2 -extra_window_size 1 -frag_type every_frame -flags +global_header -ldash 1 -utc_timing_url 'https://time.akamai.com?iso&amp;ms' -format_options 'movflags=cmaf' -timeout 0.5 -write_prft 1 -target_latency '3.0' ";
            //output config
            ffargs += "-f hls -hls_time 1 -hls_list_size 3 -hls_segment_type fmp4 -hls_flags delete_segments  live.m3u8";
            _ffmpegInfo = new ProcessStartInfo();
            _ffmpegInfo.FileName = "ffmpeg";
            Console.WriteLine(ffargs);
            //_ffmpegInfo.Arguments = "-y -f rawvideo -pix_fmt yuv420p -s:v 1536x864 -re -fflags +genpts -i - -c:v libx264 -g:v 24 -keyint_min:v 24 -sc_threshold:v 0 -preset ultrafast -tune zerolatency -use_timeline 0 -streaming 1 -window_size 2 -extra_window_size 1 -frag_type every_frame -flags +global_header -ldash 1 -utc_timing_url 'https://time.akamai.com?iso&amp;ms' -format_options 'movflags=cmaf' -timeout 0.5 -write_prft 1 -target_latency '3.0' -f hls -hls_time 1 -hls_list_size 3 -hls_segment_type fmp4 -hls_flags delete_segments  live.m3u8";
            _ffmpegInfo.Arguments = ffargs;
            _ffmpegInfo.WorkingDirectory = sdir;
            _ffmpegInfo.UseShellExecute = false;
            _ffmpegInfo.RedirectStandardInput = true;
        }
        public void StartStream()
        {
            if(_isStreamRunning) return;
            _ffmpeg = new Process();
            _ffmpeg.StartInfo = _ffmpegInfo;
            if (_streamOptions.UseRpiCamInput)
            {
                _rpicamVid = new Process();
                _rpicamVid.StartInfo = _rpicamVidInfo;
                _rpicamVid.Start();
                _ffmpeg.Start();
                _pipeTask = Task.Run(() => _pipe.Pipe(_rpicamVid.StandardOutput, _ffmpeg.StandardInput));
            } else
                _ffmpeg.Start();
            _isStreamRunning = true;
        }

        private void _stopStream()
        {
            if(_ffmpeg != null) {
                _ffmpeg.StandardInput.Close();
                Thread.Sleep(1);
                _ffmpeg.Kill();
                //_ffmpeg.Dispose();
                //_ffmpeg = null;
            }
            if(_rpicamVid != null) {
                _rpicamVid.Kill();
            }
            if(_pipeTask != null) {
                _pipeTask = null;
            }
        }
        public void StopStream()
        {
            _isStreamRunning = false;
            _stopStream();
            if (_rpicamVid != null && _ffmpeg != null && !_rpicamVid.HasExited && !_ffmpeg.HasExited)
                Task.WhenAll(_rpicamVid.WaitForExitAsync(), _ffmpeg.WaitForExitAsync()).Wait();
            else if(_ffmpeg != null && !_ffmpeg.HasExited)
                _ffmpeg.WaitForExit();
            else if(_rpicamVid != null && !_rpicamVid.HasExited)
                _rpicamVid.WaitForExit();
            if(_rpicamVid != null) {
                _rpicamVid.Dispose();
                _rpicamVid = null;
            }
            if (_ffmpeg != null)
            {
                _ffmpeg.Dispose();
                _ffmpeg = null;
            }
        }
        public async void StopStreamAsync()
        {
            _isStreamRunning = false;
            _stopStream();
            if (_rpicamVid != null && _ffmpeg != null && !_rpicamVid.HasExited && !_ffmpeg.HasExited)
                await Task.WhenAll(_rpicamVid.WaitForExitAsync(), _ffmpeg.WaitForExitAsync());
            else if (_ffmpeg != null && !_ffmpeg.HasExited)
                await _ffmpeg.WaitForExitAsync();
            else if (_rpicamVid != null && !_rpicamVid.HasExited)
                await _rpicamVid.WaitForExitAsync();
            if (_rpicamVid != null)
            {
                _rpicamVid.Dispose();
                _rpicamVid = null;
            }
            if (_ffmpeg != null)
            {
                _ffmpeg.Dispose();
                _ffmpeg = null;
            }
        }

        public bool IsStreamRunning { get { return _isStreamRunning; } }
    }

    public class VideoStreamPipe
    {
        private byte[] buf = new byte[5308416];
        public void Pipe(StreamReader sin, StreamWriter sout)
        {
            using (Stream sr = sin.BaseStream)
            using (Stream sw = sout.BaseStream)
            {
                while (true)
                {
                    try
                    {
                        var bcnt = sr.ReadAtLeast(buf.AsSpan(), 3072);
                        sw.Write(buf, 0, bcnt);
                    }
                    catch (EndOfStreamException ex)
                    {
                        return;
                    }
                }
            }
        }

        public Task Run(StreamReader sin, StreamWriter sout)
        {
            return Task.Run(()=>Pipe(sin, sout));
        }
    }
}


