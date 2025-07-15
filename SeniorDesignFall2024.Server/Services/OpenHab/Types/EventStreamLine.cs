namespace SeniorDesignFall2024.Server.Services.OpenHab.Types
{
    public class EventStreamLine : EventStreamAbstract
    {
        public EventStreamLineType type { get; set; }
        public int retry { get; set; } = 0;
        public string? data { get; set; }
    }
}
