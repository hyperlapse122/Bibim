using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using HyperLapse.Bibim.Service.YoutubeDL.Models;
using Microsoft.Extensions.Logging;
using YoutubeDLSharp.Metadata;

namespace HyperLapse.Bibim.Service.YoutubeDL.Service;

internal class YouTubeDLAudioQueueService(
    YoutubeDLSharp.YoutubeDL client,
    IAudioQueueService audioQueueService,
    ILogger<YouTubeDLAudioQueueService> logger,
    YouTubeDLOptions options) : IYouTubeDLAudioQueueService
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
        var video = res.Data;
        var title = video.Title;
        var uploader = video.Uploader;
        var views = video.ViewCount;
// all available download formats
        FormatData[] formats = video.Formats;
// ...

        var item = new YouTubeDLAudioQueueItem(options, url, logger)
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