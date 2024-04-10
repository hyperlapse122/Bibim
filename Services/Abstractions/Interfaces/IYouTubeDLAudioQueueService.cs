namespace HyperLapse.Bibim.Service.Abstractions.Interfaces;

public interface IYouTubeDLAudioQueueService
{
    public Task<IAudioQueueItem> EnqueueAsync(
        ulong channelId,
        string url,
        CancellationToken cancellationToken = default,
        TaskCompletionSource? taskCompletionSource = null
    );
}