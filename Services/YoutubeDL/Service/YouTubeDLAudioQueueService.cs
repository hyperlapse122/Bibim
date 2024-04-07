using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using HyperLapse.Bibim.Service.YoutubeDL.Models;
using Microsoft.Extensions.Logging;
using YoutubeDLSharp.Metadata;

namespace HyperLapse.Bibim.Service.YoutubeDL.Service;

public class YouTubeDLAudioQueueService(
    YoutubeDLSharp.YoutubeDL client,
    IAudioQueueService audioQueueService,
    ILogger<YouTubeDLAudioQueueService> logger)
{
    public async Task<IAudioQueueItem> EnqueueAsync(
        ulong channelId,
        string url,
        CancellationToken cancellationToken = default,
        TaskCompletionSource? taskCompletionSource = null
    )
    {
        var res = await client.RunVideoDataFetch(url, cancellationToken);
// get some video information
        VideoData video = res.Data;
        string title = video.Title;
        string uploader = video.Uploader;
        long? views = video.ViewCount;
// all available download formats
        FormatData[] formats = video.Formats;
// ...

        var item = new YouTubeDLAudioQueueItem(client: client, url: url, logger: logger)
        {
            SourceDisplayName = "YoutubeDL",
            DisplayName = title,
            TaskCompletionSource = taskCompletionSource,
            CancellationToken = cancellationToken
        };
        await audioQueueService.EnqueueAsync(channelId, item, cancellationToken);

        return item;
    }
}