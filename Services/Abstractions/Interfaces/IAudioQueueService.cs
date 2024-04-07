namespace HyperLapse.Bibim.Service.Abstractions.Interfaces;

public interface IAudioQueueService
{
    public Task EnqueueAsync(ulong channelId, IAudioQueueItem item, CancellationToken cancellationToken = default);
    public Task<IAudioQueueItem?> PeekAsync(ulong channelId, CancellationToken cancellationToken = default);
    public Task<IAudioQueueItem> DequeueAsync(ulong channelId, CancellationToken cancellationToken = default);
    public Task<IAudioQueueItem> RemoveFromQueueAsync(ulong channelId, CancellationToken cancellationToken = default);
}