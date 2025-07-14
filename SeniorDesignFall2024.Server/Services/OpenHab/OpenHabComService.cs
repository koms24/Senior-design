using SeniorDesignFall2024.Server.Services.OpenHab.Types;
using System.Threading.Channels;

namespace SeniorDesignFall2024.Server.Services.OpenHab
{
    public class OpenHabComService
    {
        private class TimerParams
        {
            public TimerCallback Callback { get; set; }
            public object? State { get; set; } = null;
            public TimeSpan DueTIme { get; set; }
            public TimeSpan Period { get; set; }
        }
        private Channel<EventStreamLine> openHabEvents = Channel.CreateBounded<EventStreamLine>(new BoundedChannelOptions(64) { FullMode = BoundedChannelFullMode.DropOldest, SingleWriter = true });
        private Timer? eventStreamTimeoutTimer = null;
        private TimerParams? timerParams = null;

        public void ConfigTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            timerParams = new TimerParams
            {
                Callback = callback,
                State = state,
                DueTIme = dueTime,
                Period = period
            };
        }
        public void StartTimer()
        {
            if (timerParams == null)
                throw new ArgumentNullException(nameof(timerParams));
            eventStreamTimeoutTimer = new Timer(timerParams.Callback, timerParams.State, timerParams.DueTIme, timerParams.Period);
        }
        public void ResetTimer()
        {
            if (timerParams == null)
                throw new ArgumentNullException(nameof(timerParams));
            if (eventStreamTimeoutTimer == null)
                throw new ArgumentNullException(nameof(eventStreamTimeoutTimer));
            eventStreamTimeoutTimer.Change(timerParams.DueTIme, timerParams.Period);
        }

        public async Task ReceivedEventFromOpenHab(EventStreamLine evt) { 
            await openHabEvents.Writer.WriteAsync(evt);
        }

        public ChannelReader<EventStreamLine> EventChannelReader { get { return openHabEvents.Reader; } }
    }
}
