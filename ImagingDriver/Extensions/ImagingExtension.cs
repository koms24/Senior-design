using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SeniorDesignFall2024.ImagingDriver.Options;
using SeniorDesignFall2024.ImagingDriver.SDCam;
using SeniorDesignFall2024.ImagingDriver.Services;
using SeniorDesignFall2024.Server.Shared.Extensions;
using SeniorDesignFall2024.Server.Shared.Interfaces;
using SeniorDesignFall2024.Server.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorDesignFall2024.ImagingDriver.Extensions
{
    public static class ImagingExtension
    {
        public static void AddImagingDriver(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<StreamOptions>(builder.Configuration.GetSection(StreamOptions.SectionName));
            IStartupConfigOptions opts = builder.GetStartupConfigOptions();
            if (opts?.EnableImaging ?? false)
            {
                var services = builder.Services;
                if (opts?.EnableCamera ?? false)
                    services.AddSingleton<SDCamController>();
                services.AddSingleton<VideoStreamControllerService>();
                if (opts?.EnableCamera ?? false)
                    services.AddSingleton<StillCaptureControllerService>();
            }
        }
    }
}
