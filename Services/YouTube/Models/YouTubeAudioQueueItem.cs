using System.IO.Pipelines;
using HyperLapse.Bibim.Service.Abstractions.EventArgs;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace HyperLapse.Bibim.Service.YouTube.Models;

internal class YouTubeAudioQueueItem(
    YoutubeClient client,
    IVideo video,
    TaskCompletionSource? taskCompletionSource = null,
    CancellationToken cancellationToken = default
) : IAudioQueueItem
{
    public event EventHandler<AudioQueueItemStateChangedEventArgs>? StateChanged;
    public string SourceDisplayName => "YouTube";
    public string DisplayName => video.Title;
    public CancellationToken CancellationToken { get; } = cancellationToken;
    public TaskCompletionSource? TaskCompletionSource { get; } = taskCompletionSource;

    public async Task<(Pipe, Task)> GetAudioPipeAsync(CancellationToken cancellationToken)
    {
        var pipe = new Pipe();

        var manifest = await client.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
        var streamInfo = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        var stream = await client.Videos.Streams.GetAsync(streamInfo, cancellationToken);
        var task = Task.Run(async () =>
        {
            await using var writeStream = pipe.Writer.AsStream();
            await stream.CopyToAsync(writeStream, cancellationToken);
        }, cancellationToken);
        return (pipe, task);
    }
}