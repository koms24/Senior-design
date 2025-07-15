using SeniorDesignFall2024.ImagingDriver.Extensions;
using SeniorDesignFall2024.Server.Extensions;
using SeniorDesignFall2024.Server.Shared.Extensions;
using SeniorDesignFall2024.Server.Formatter;
using SeniorDesignFall2024.Server.Services;
using SeniorDesignFall2024.TmcDriver;
using SeniorDesignFall2024.TmcDriver.DI;
using SeniorDesignFall2024.TmcDriver.Extensions;
using SeniorDesignFall2024.Server.Services.OpenHab;
using SeniorDesignFall2024.Database.Extension;

namespace SeniorDesignFall2024.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddStartupConfig();
            builder.AddSdDbContextOptions();

            builder.Services.AddSingleton<FileStreamerService>();

            builder.AddGpioController();
            builder.AddTmcDriver();

            // Add services to the container.
            builder.AddHlsStaticFiles();
            builder.AddImagingDriver();
            builder.AddOpenHabBase();

            builder.AddSdDbContext();

            builder.AddBackgroundDataLogger();
            builder.AddOpenHabProcesses();

            builder.Services.AddControllers(options=>options.InputFormatters.Add(new PlainTextFormatter()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.SyncOpenHabToDb();

            app.UseDefaultFiles();
            app.UseHlsStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
