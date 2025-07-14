namespace SeniorDesignFall2024.Server.Services.OpenHab
{
    public class OpenHabBackgroundService : BackgroundService, IDisposable
    {
        private OpenHabService _openHabService;
        private OpenHabComService _openHabComService;
        private OpenHabEventService _openHabEventService;

        private CancellationTokenSource _cancelEventService = new CancellationTokenSource();
        private CancellationTokenSource _cancel = new CancellationTokenSource();

        private bool _streamTimedout = false;

        private OHStateMessageProcessorComService _msgProcessorsCom;
        private OHStateMessageProcessor[] _msgProcessors = [];
        private Task[] _msgProcessorTasks = [];

        public OpenHabBackgroundService(
            OpenHabComService openHabComService,
            OHStateMessageProcessorComService msgProcessorsCom,
            OpenHabEventService openHabEventService,
            OpenHabService openHabService
            )
        {
            _openHabService = openHabService;
            _msgProcessorsCom = msgProcessorsCom;
            _openHabComService = openHabComService;
            _openHabEventService = openHabEventService;

            _openHabComService.ConfigTimer(OnTimeout, null, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancel = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, _cancelEventService.Token);
            var msgProcessors = RunMessageProcessors(1, stoppingToken);
            var evtService = RunEventService(stoppingToken);
            await Task.WhenAll(msgProcessors, evtService);
        }

        protected async Task RunEventService(CancellationToken stoppingToken)
        {
            var msgProcessors = RunMessageProcessors(1, stoppingToken);
            do
            {
                _streamTimedout = false;
                var stream = await _openHabService.GetEventStreamAsync();
                await _openHabEventService.Run(stream, stoppingToken);
                _openHabEventService.CloseStream();
            } while (!stoppingToken.IsCancellationRequested);
        }

        protected async Task RunMessageProcessors(int processorCnt, CancellationToken cancellationToken)
        {
            if(_msgProcessorTasks.Length > 0)
                foreach(var t in _msgProcessorTasks)
                    t.Dispose();
            _msgProcessors = Enumerable.Range(0, processorCnt).Select(i=>new OHStateMessageProcessor(_msgProcessorsCom)).ToArray();
            _msgProcessorTasks = _msgProcessors.Select(o => o.Run(cancellationToken)).ToArray();
            await Task.WhenAll(_msgProcessorTasks);
        }

        private void OnTimeout(object? state)
        {
            _streamTimedout = true;
            _cancelEventService.Cancel();
        }

        public void Dispose()
        {
            if (!_cancelEventService.IsCancellationRequested)
                _cancelEventService.Cancel();
            _openHabEventService?.Dispose();
            base.Dispose();
        }
    }
}
