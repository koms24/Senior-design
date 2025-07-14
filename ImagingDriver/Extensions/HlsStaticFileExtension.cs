using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using SeniorDesignFall2024.ImagingDriver.Options;

namespace SeniorDesignFall2024.ImagingDriver.Extensions
{
    public static class HlsStaticFilesExtension
    {
        public static IServiceCollection AddHlsStaticFiles(this WebApplicationBuilder builder)
        {
            return builder.Services.Configure<HlsOptions>(builder.Configuration.GetSection(HlsOptions.SectionName));
        }
        public static IApplicationBuilder UseHlsStaticFiles(this WebApplication app)
        {
            var opts = app.Services.GetRequiredService<IOptions<HlsOptions>>().Value;
            return app.UseHlsStaticFiles(opts.StreamFolderPath, opts.ApiRequestPath);
        }
        public static IApplicationBuilder UseHlsStaticFiles(this WebApplication app, string folderPath, string? requestPath)
        {
            string path;
            string rPath = requestPath ?? Path.GetFileName(Path.TrimEndingDirectorySeparator(folderPath));
            if (Path.IsPathRooted(folderPath)) {
                if (!Directory.Exists(folderPath)) {
                    throw new DirectoryNotFoundException(folderPath);
                }
                path = folderPath;
            } else {
                path = Path.Combine(app.Environment.ContentRootPath, folderPath);
            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            var fileExtContentTypeProvider = new FileExtensionContentTypeProvider();
            fileExtContentTypeProvider.Mappings[".m3u8"] = "application/x-mpegURL";
            fileExtContentTypeProvider.Mappings[".mp4"] = "video/mp4";
            fileExtContentTypeProvider.Mappings[".ts"] = "video/MP2T";
            fileExtContentTypeProvider.Mappings[".m4s"] = "video/iso.segment";
            fileExtContentTypeProvider.Mappings["m4v"] = "video/mp4";
            fileExtContentTypeProvider.Mappings["m4a"] = "audio/mp4";

            return app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = rPath,
                ContentTypeProvider = fileExtContentTypeProvider,
                FileProvider = new PhysicalFileProvider(path)
            });
        }
    }
}
