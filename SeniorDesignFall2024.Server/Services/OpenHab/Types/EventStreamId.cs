namespace SeniorDesignFall2024.Server.Services.OpenHab.Types
{
    public class EventStreamId : EventStreamLine
    {
        public string Id { get { return data ?? ""; } }
    }
}
