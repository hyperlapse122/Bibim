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

    public async Task<Stream> GetAudioStreamAsync(CancellationToken cancellationToken)
    {
        var manifest = await client.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
        var streamInfo = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        var stream = await client.Videos.Streams.GetAsync(streamInfo, cancellationToken);
        return stream;
    }
}