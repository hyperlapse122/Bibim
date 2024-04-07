using System.Threading.Channels;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;

namespace HyperLapse.Bibim.Service.AudioQueue.Services;

internal class AudioQueueService : IAudioQueueService
{
    private readonly Dictionary<long, Channel<IAudioQueueItem>> _channel = new();

    private readonly BoundedChannelOptions _options = new(100)
    {
        SingleReader = true,
        SingleWriter = false,
        FullMode = BoundedChannelFullMode.Wait
    };

    public async Task EnqueueAsync(long channelId, IAudioQueueItem item, CancellationToken cancellationToken = default)
    {
        await GetChannel(channelId).Writer.WriteAsync(item, cancellationToken);
    }

    public Task<IAudioQueueItem?> PeekAsync(long channelId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task<IAudioQueueItem> DequeueAsync(long channelId, CancellationToken cancellationToken = default)
    {
        return await GetChannel(channelId).Reader.ReadAsync(cancellationToken);
    }

    public Task<IAudioQueueItem> RemoveFromQueueAsync(long channelId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    private Channel<IAudioQueueItem> GetChannel(long channelId)
    {
        if (_channel.TryGetValue(channelId, out var channel)) return channel;

        channel = Channel.CreateBounded<IAudioQueueItem>(_options);
        _channel[channelId] = channel;

        return channel;
    }
}