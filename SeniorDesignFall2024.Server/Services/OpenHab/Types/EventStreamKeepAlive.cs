using System.Text.Json;

namespace SeniorDesignFall2024.Server.Services.OpenHab.Types
{
    public class EventStreamKeepAlive : EventStreamData<OpenHabKeepAliveEvent>
    {
        public string Type { get { return TypedData.type; } }
        public int Interval { get { return TypedData.interval; } }

        public static EventStreamKeepAlive Parse(EventStreamLine obj)
        {
            return new EventStreamKeepAlive
            {
                type = obj.type,
                data = obj.data,
                retry = obj.retry,
                TypedData = JsonSerializer.Deserialize<OpenHabKeepAliveEvent>(obj.data ?? "{}") ?? throw new FormatException()
            };
        }
    }
}
