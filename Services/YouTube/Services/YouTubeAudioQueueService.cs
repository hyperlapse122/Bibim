using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using HyperLapse.Bibim.Service.YouTube.Models;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace HyperLapse.Bibim.Service.YouTube.Services;

public class YouTubeAudioQueueService(YoutubeClient client, IAudioQueueService audioQueueService)
{
    public async Task<IAudioQueueItem> Enqueue(
        ulong channelId,
        string uri,
        CancellationToken cancellationToken = default,
        TaskCompletionSource? taskCompletionSource = null
    )
    {
        var id = VideoId.Parse(uri);
        var video = await client.Videos.GetAsync(id, cancellationToken);

        var item = new YouTubeAudioQueueItem(client, video, taskCompletionSource, cancellationToken);
        await audioQueueService.EnqueueAsync(channelId, item, cancellationToken);

        return item;
    }
}