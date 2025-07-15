using SeniorDesignFall2024.Database.Model.OpenHab.Entity;
using SeniorDesignFall2024.Server.Services.OpenHab.Types;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading.Channels;

namespace SeniorDesignFall2024.Server.Services.OpenHab
{
    public class OHStateMessageProcessorComService
    {
        public delegate Task SubscriberCallback(IEnumerable<KeyValuePair<string, StateType>> vals);

        private ConcurrentDictionary<SubscriberCallback, string[]> subscribers = new();
        private ConcurrentDictionary<string, ImmutableHashSet<SubscriberCallback>> mapToSubscribers = new();
        public ConcurrentDictionary<string, ImmutableHashSet<SubscriberCallback>> SubscribersMap { get { return mapToSubscribers; } }


        private ChannelReader<EventStreamLine> reader;
        public ChannelReader<EventStreamLine> Reader { get { return reader; } }

        public OHStateMessageProcessorComService(OpenHabComService comService) {
            this.reader = comService.EventChannelReader;
        }

        public void Subscribe(SubscriberCallback cb, string item_uid)
        {
            mapToSubscribers.AddOrUpdate(item_uid, (k) => ImmutableHashSet.Create(cb), (k, v) => v.Contains(cb) ? v : ImmutableHashSet.Create<SubscriberCallback>(v.AsEnumerable().Append(cb).ToArray()) );
        }

        public void Unsubscribe(SubscriberCallback cb, string item_uid)
        {
            if(mapToSubscribers.ContainsKey(item_uid))
                mapToSubscribers.AddOrUpdate(
                    item_uid,
                    (k) => ImmutableHashSet.Create<SubscriberCallback>(),
                    (k, v) => v.Contains(cb) ? v.Remove(cb) : v 
                );
        }
    }
}
