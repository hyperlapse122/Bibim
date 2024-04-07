using CliWrap;
using HyperLapse.Bibim.Service.Abstractions.EventArgs;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;

namespace HyperLapse.Bibim.Service.YoutubeDL.Models;

internal class YouTubeDLAudioQueueItem(YoutubeDLSharp.YoutubeDL client, string url, ILogger logger)
    : IAudioQueueItem
{
    public required string SourceDisplayName { get; internal set; }
    public required string DisplayName { get; internal set; }
    public CancellationToken CancellationToken { get; internal set; }
    public TaskCompletionSource? TaskCompletionSource { get; internal set; }
    public event EventHandler<AudioQueueItemStateChangedEventArgs>? StateChanged;

    public async Task<Stream> GetAudioStreamAsync(CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();

        await Cli.Wrap("yt-dlp")
            .WithArguments(
                $"\"{url}\" -o -"
            )
            .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(e => logger.LogInformation("{message}", e)))
            .ExecuteAsync(cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}