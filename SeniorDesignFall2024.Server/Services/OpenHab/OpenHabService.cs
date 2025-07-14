using Microsoft.Extensions.Options;
using SeniorDesignFall2024.Server.Options;
using SeniorDesignFall2024.Server.Services.OpenHab.Types;
using System.Collections.Concurrent;

namespace SeniorDesignFall2024.Server.Services.OpenHab
{
    public class OpenHabService
    {
        protected Func<string?>? connectionStringGetter = null;
        protected ConcurrentBag<string> _sseState = new();

        private IHttpClientFactory _httpClientFactory;
        private HttpClient? _httpClient = null;

        protected HttpClient HttpClient { get { return _httpClient != null ? _httpClient : (_httpClient = _httpClientFactory.CreateClient(Extensions.OpenHabServiceExtension.OpenHabHttpClient)); } }

        public OpenHabService(IHttpClientFactory httpClientFactory, IOptions<OpenHabConfigOptions> opts)
        {
            _httpClientFactory = httpClientFactory;
            var uids = opts.Value.ItemSubscriptions;
            foreach(var uid in uids)
                _sseState.Add(uid);
        }

        public async Task<Stream> GetEventStreamAsync()
        {
            return await HttpClient.GetStreamAsync("events/states");
        }

        public async Task SendItemCommand(string item_name, string command, CancellationToken? token = null) {
            using StringContent cmd = new(command, System.Text.Encoding.UTF8, "text/plain");
            using HttpResponseMessage response = await HttpClient.PostAsync($"items/{item_name}", cmd, token??CancellationToken.None);
            response.EnsureSuccessStatusCode();
        }

        public async Task<EnrichedItemDto> GetItem(string item_name, CancellationToken? token = null) {
            var item = await HttpClient.GetFromJsonAsync<EnrichedItemDto>($"items/{item_name}", token ?? CancellationToken.None);
            if(item == null)
                throw new ArgumentNullException(nameof(item));
            return item;
        }

        public async Task UpdateEventFilterList(Func<string?> getConnectionString, CancellationToken cancellationToken)
        {
            string? connection_id = getConnectionString();
            if (connection_id != null)
                connectionStringGetter = getConnectionString;
            if(connection_id == null)
                throw new ArgumentNullException(nameof(connection_id));
            var response = await HttpClient.PostAsJsonAsync($"events/states/{connection_id}", _sseState.ToArray(), cancellationToken);
        }

        public async Task AddEventFilter(string filterStr, CancellationToken cancellationToken)
        {
            _sseState.Add(filterStr);
            if(connectionStringGetter != null)
                await UpdateEventFilterList(connectionStringGetter, cancellationToken);
        }
    }
}
