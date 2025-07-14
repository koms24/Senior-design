#include <chrono>
#include <filesystem>
#include <poll.h>
#include <signal.h>
#include <sys/signalfd.h>
#include <sys/stat.h>
#include <iostream>

#include <core/rpicam_app.hpp>
#include <core/options.hpp>

#include <image/image.hpp>

#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/highgui.hpp>

using namespace std::placeholders;
using libcamera::Stream;

class SDCamController
{
public:
	SDCamController()
	{
		auto opts = app.GetOptions();
		opts->Parse(0, NULL);
		opts->denoise = "auto";
		opts->no_raw = true;
		opts->nopreview = true;
	}

	cv::Mat getMatStillFromCamera()
	{
		app.OpenCamera();
		app.ConfigureStill(RPiCamApp::FLAG_STILL_BGR);
		app.StartCamera();

		cv::Mat img;
		for (;;)
		{
			RPiCamApp::Msg msg = app.Wait();
			if (msg.type == RPiCamApp::MsgType::Timeout)
			{
				LOG_ERROR("ERROR: Device timeout detected, attempting a restart!!!");
				app.StopCamera();
				app.StartCamera();
				continue;
			}

			if (msg.type != RPiCamApp::MsgType::RequestComplete)
				throw std::runtime_error("unrecognised message!");

			app.StopCamera();
			//LOG(1, "Still capture image received");

			Stream *stream = app.StillStream();
			StreamInfo info = app.GetStreamInfo(stream);
			CompletedRequestPtr &payload = std::get<CompletedRequestPtr>(msg.payload);
			BufferReadSync r(&app, payload->buffers[stream]);
			libcamera::Span<uint8_t> buffer = r.Get()[0];
			uint8_t *ptr = (uint8_t *)buffer.data();
			cv::Mat image(info.height, info.width, CV_8UC3, ptr);
			img = image.clone();
			break;
		}
		app.Teardown();
		app.CloseCamera();
		return img;
	}

    void debugImshow(cv::Mat img) {
        cv::Mat img_;
		cv::resize(img, img_, cv::Size(), 0.25, 0.25, cv::INTER_AREA);
		cv::imshow("test", img_);
		cv::waitKey();
		cv::destroyAllWindows();
    }
protected:
	RPiCamApp app;
};