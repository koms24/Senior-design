using System.Text.Json;

namespace SeniorDesignFall2024.Server.Services.OpenHab.Types
{
    public class EventStreamStates : EventStreamData<IDictionary<string, OpenHabState>>
    {
        public OpenHabState GetItemState(string itemUid)
        {
            return TypedData[itemUid];
        }
        public bool HasItem(string itemUid)
        {
            return TypedData.ContainsKey(itemUid);
        }
        public OpenHabState? GetItemStateOrNull(string itemUid)
        {
            return TypedData.ContainsKey(itemUid) ? TypedData[itemUid] : null;
        }

        public static EventStreamStates Parse(EventStreamLine obj)
        {
            return new EventStreamStates
            {
                type = obj.type,
                data = obj.data,
                retry = obj.retry,
                TypedData = JsonSerializer.Deserialize<Dictionary<string, OpenHabState>>(obj.data ?? "{}") ?? throw new FormatException()
            };
        }
    }
}
