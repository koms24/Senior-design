namespace SeniorDesignFall2024.Server.Services.OpenHab.Types
{
    public class EventStreamEvent : EventStreamLine
    {
        public string Name { get { return data?.Trim() ?? ""; } }
        public bool IsAlive { get { return Name == "alive"; } }
    }
}
