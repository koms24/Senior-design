using SeniorDesignFall2024.Database.Model.OpenHab.Entity;
using SeniorDesignFall2024.Server.Services.OpenHab.Types;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading.Channels;

namespace SeniorDesignFall2024.Server.Services.OpenHab
{
    public class OHStateMessageProcessor
    {
        private OHStateMessageProcessorComService comService;
        private readonly static JsonSerializerOptions jopts = new JsonSerializerOptions {
            AllowOutOfOrderMetadataProperties = true
        };
        public OHStateMessageProcessor(OHStateMessageProcessorComService comService) {
            this.comService = comService;
        }

        protected static ConcurrentDictionary<string, StateType> convertToStateDictionary(EventStreamLine evtMsg)
        {
            if (evtMsg.data == null)
                return new();
            return JsonSerializer.Deserialize<ConcurrentDictionary<string, StateType>>(evtMsg.data, jopts) ?? new();
        }

        public async Task Run(CancellationToken cancellationToken) {
            ChannelReader<EventStreamLine> reader = comService.Reader;
            do
            {
                EventStreamLine? msg;
                try
                {
                    msg = await reader.ReadAsync(cancellationToken);
                } catch(ChannelClosedException ex) when (ex.InnerException == null) {
                    return;
                } catch (ChannelClosedException ex) { 
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                if (cancellationToken.IsCancellationRequested)
                    break;
                else if (msg == null)
                    continue;
                ConcurrentDictionary<string, StateType> dat = convertToStateDictionary(msg);
                if(dat.Count > 0)
                    await dispatchEventStates(dat, cancellationToken);
            } while (!cancellationToken.IsCancellationRequested);
        }

        protected struct CbTriple
        {
            public OHStateMessageProcessorComService.SubscriberCallback callback;
            public KeyValuePair<string, StateType> kp;
            public CbTriple(
                string id,
                StateType st,
                OHStateMessageProcessorComService.SubscriberCallback cb
                )
            {
                callback = cb;
                kp = KeyValuePair.Create(id, st);
            }
        }
        protected async Task dispatchEventStates(IReadOnlyDictionary<string, StateType> states, CancellationToken cancellationToken)
        {
            var dispatchTasks = comService.SubscribersMap.AsReadOnly()
                .Where(kp => states.ContainsKey(kp.Key))
                .SelectMany(kp => kp.Value, (kp, cb) => new CbTriple(kp.Key, states[kp.Key], cb))
                .GroupBy(ct => ct.callback, o => o.kp)
                .Select(o => o.Key(o.AsEnumerable()));
            var waitAll = Task.WhenAll(dispatchTasks);
            await waitAll;
        }
    }
}
