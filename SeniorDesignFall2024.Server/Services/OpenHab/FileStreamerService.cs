
using System.Threading.Channels;

namespace SeniorDesignFall2024.Server.Services.OpenHab
{
    public class FileStreamerService : BackgroundService
    {
        protected class StreamableFile
        {
            public Action<Stream> Action { get; set; }
            public Stream Stream { get; set; }
        }

        protected Channel<StreamableFile> _channel = Channel.CreateBounded<StreamableFile>(10);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Run(stoppingToken);
        }

        protected async Task Run(CancellationToken cancellationToken)
        {
            var reader = _channel.Reader;
            do
            {
                StreamableFile file = await reader.ReadAsync(cancellationToken);
                StreamFile(file, cancellationToken);
            } while(!cancellationToken.IsCancellationRequested);
        }

        protected async void StreamFile(StreamableFile f, CancellationToken cancellationToken)
        {
            await Task.Run(() => f.Action(f.Stream), cancellationToken);
        }

        public async Task QueueFileStream(Action<Stream> action, Stream stream, CancellationToken cancellationToken = default)
        {
            await _channel.Writer.WriteAsync(new StreamableFile { Action = action, Stream = stream }, cancellationToken);
        }
    }
}
