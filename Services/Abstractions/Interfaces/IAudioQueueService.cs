namespace HyperLapse.Bibim.Service.Abstractions.Interfaces;

public interface IAudioQueueService
{
    public Task EnqueueAsync(long channelId, IAudioQueueItem item, CancellationToken cancellationToken = default);
    public Task<IAudioQueueItem?> PeekAsync(long channelId, CancellationToken cancellationToken = default);
    public Task<IAudioQueueItem> DequeueAsync(long channelId, CancellationToken cancellationToken = default);
    public Task<IAudioQueueItem> RemoveFromQueueAsync(long channelId, CancellationToken cancellationToken = default);
}