using System.Threading.Channels;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;

namespace HyperLapse.Bibim.Service.AudioQueue.Services;

internal class AudioQueueService : IAudioQueueService
{
    private readonly Dictionary<ulong, Channel<IAudioQueueItem>> _channel = new();

    private readonly BoundedChannelOptions _options = new(100)
    {
        SingleReader = true,
        SingleWriter = true,
        FullMode = BoundedChannelFullMode.Wait
    };

    public async Task EnqueueAsync(ulong channelId, IAudioQueueItem item, CancellationToken cancellationToken = default)
    {
        var channel = GetChannel(channelId);
        var channelWriter = channel.Writer;
        await channelWriter.WriteAsync(item, cancellationToken);
    }

    public Task<IAudioQueueItem?> PeekAsync(ulong channelId, CancellationToken cancellationToken = default)
    {
        return GetChannel(channelId).Reader.TryPeek(out var item)
            ? Task.FromResult<IAudioQueueItem?>(item)
            : Task.FromResult<IAudioQueueItem?>(null);
    }

    public async Task<IAudioQueueItem> DequeueAsync(ulong channelId, CancellationToken cancellationToken = default)
    {
        return await GetChannel(channelId).Reader.ReadAsync(cancellationToken);
    }

    public Task<IAudioQueueItem> RemoveFromQueueAsync(ulong channelId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    private Channel<IAudioQueueItem> GetChannel(ulong channelId)
    {
        if (_channel.TryGetValue(channelId, out var channel)) return channel;

        channel = Channel.CreateBounded<IAudioQueueItem>(_options);
        _channel[channelId] = channel;

        return channel;
    }
}