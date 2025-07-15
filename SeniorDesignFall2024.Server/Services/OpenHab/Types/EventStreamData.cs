namespace SeniorDesignFall2024.Server.Services.OpenHab.Types
{
    public class EventStreamData<T> : EventStreamLine
    {
        public T TypedData { get; set; }
    }
}
