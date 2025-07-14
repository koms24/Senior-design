using SeniorDesignFall2024.Server.Shared.Interfaces;

namespace SeniorDesignFall2024.Server.Shared.Options
{
    public class StartupConfigOptions : IStartupConfigOptions
    {
        public const string SectionName = "StartupConfig";

        public bool EnableZwaveOpenHab { get; set; } = true;
        public bool EnableTmcDriver { get; set; } = true;
        public bool EnableDb {  get; set; } = true;
        public bool EnableGpio { get; set; } = true;
        public bool EnableImaging { get; set; } = true;
        public bool EnableCamera { get; set; } = true;
    }
}
