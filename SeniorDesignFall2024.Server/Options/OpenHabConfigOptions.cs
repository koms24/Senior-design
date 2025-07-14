namespace SeniorDesignFall2024.Server.Options
{
    public class OpenHabConfigOptions
    {
        public const string SectionName = "OpenHabConfig";

        public string ApiEndpoint { get; set; } = "http://localhost:8080";
        public string[] ItemSubscriptions { get; set; } = [];
    }
}
