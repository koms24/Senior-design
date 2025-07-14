using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SeniorDesignFall2024.Database.Model.OpenHab.Context;
using SeniorDesignFall2024.Database.Model.OpenHab.Entity;
using SeniorDesignFall2024.Server.Options;
using SeniorDesignFall2024.Server.Services.OpenHab;
using System.Threading.Channels;

namespace SeniorDesignFall2024.Server.Services
{
    public class DataLoggerBackgroundService : BackgroundService
    {
        private class DataWrapper
        {
            public required string Uid;
            public StateType? OHState;
            public object? obj;
        }
        private Channel<IEnumerable<DataWrapper>> _inData = Channel.CreateBounded<IEnumerable<DataWrapper>>(100);
        private IServiceScopeFactory _scopeFactory;
        public DataLoggerBackgroundService(
            OHStateMessageProcessorComService comService,
            IOptions<OpenHabConfigOptions> opts,
            IServiceScopeFactory scopeFactory
            )
        {
            var uids = opts.Value.ItemSubscriptions;
            foreach (var uid in uids)
                comService.Subscribe(ReceivedOHStateDataAsync, uid);
            _scopeFactory = scopeFactory;
        }

        public async Task ReceivedDataAsync<T>(IEnumerable<KeyValuePair<string, T>> vals)
        {
            if(typeof(T).IsAssignableTo(typeof(StateType)))
                await _inData.Writer.WriteAsync(vals.Select(o => new DataWrapper { Uid = o.Key, OHState = o.Value as StateType, obj = null }).ToArray());
            else
                await _inData.Writer.WriteAsync(vals.Select(o => new DataWrapper { Uid = o.Key, OHState = null, obj = o.Value }).ToArray());
        }

        public async Task ReceivedOHStateDataAsync(IEnumerable<KeyValuePair<string, StateType>> vals)
        {
            await ReceivedDataAsync<StateType>(vals);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                await Run(stoppingToken);
                return;
            } while (!stoppingToken.IsCancellationRequested);
        }

        protected async Task Run(CancellationToken cancellationToken)
        {
            do
            {
                var data = await _inData.Reader.ReadAsync(cancellationToken);
                await _logToDb(data, cancellationToken);
            } while (!cancellationToken.IsCancellationRequested);
        }

        private async Task _logToDb(IEnumerable<DataWrapper> dat, CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SdDbContext>();
                var uids = dat.Select(o => o.Uid).ToHashSet();
                var itemSet = db.Set<Item>();
                var stateSet = db.Set<StateType>();
                var items = await itemSet.Where(i => uids.Contains(i.Uid)).ToDictionaryAsync(o=>o.Uid, cancellationToken);
                var newStates = dat.Where(o => o.OHState != null).Select(o =>
                {
                    var s = o.OHState;
                    s.ParentItemId = o.Uid;
                    stateSet.Add(s);
                    items[o.Uid].State = s;
                    return s;
                }).ToArray();
                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
