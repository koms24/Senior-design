namespace SeniorDesignFall2024.ImagingDriver.Options
{
    public class HlsOptions
    {
        public const string SectionName = "HlsConfig";

        public string StreamFolderPath { get; set; } = "Stream";
        public string ApiRequestPath { get; set; } = "/Stream";
    }
}
