using System.IO.Pipelines;
using CliWrap;
using HyperLapse.Bibim.Service.Abstractions.EventArgs;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;

namespace HyperLapse.Bibim.Service.YoutubeDL.Models;

internal class YouTubeDLAudioQueueItem(YouTubeDLOptions options, string url, ILogger logger)
    : IAudioQueueItem
{
    public required string SourceDisplayName { get; internal set; }
    public required string DisplayName { get; internal set; }
    public CancellationToken CancellationToken { get; internal set; }
    public TaskCompletionSource? TaskCompletionSource { get; internal set; }
    public event EventHandler<AudioQueueItemStateChangedEventArgs>? StateChanged;

    public async Task GetAudioPipeAsync(PipeWriter writer,CancellationToken cancellationToken)
    {
        await using var writeStream = writer.AsStream();

        await Cli.Wrap(options.YoutubeDLPath)
            .WithArguments($"\"{url}\" -o -")
            .WithStandardOutputPipe(PipeTarget.ToStream(writeStream))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(e => logger.LogInformation("{message}", e)))
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .ExecuteAsync(cancellationToken);
    }
}