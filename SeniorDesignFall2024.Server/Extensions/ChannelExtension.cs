using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace SeniorDesignFall2024.Server.Extensions
{
    public static class ChannelExtension
    {
        public static async IAsyncEnumerable<IEnumerable<T>> ReadBatchesAsync<T>(
            this ChannelReader<T> reader,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
        {
            while (await reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return reader.Flush().ToList();
            }
        }

        public static IEnumerable<T> Flush<T>(this ChannelReader<T> reader)
        {
            while (reader.TryRead(out T item))
            {
                yield return item;
            }
        }
    }
}
