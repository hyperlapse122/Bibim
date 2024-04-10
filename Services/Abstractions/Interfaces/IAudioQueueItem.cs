using System.IO.Pipelines;
using HyperLapse.Bibim.Service.Abstractions.EventArgs;

namespace HyperLapse.Bibim.Service.Abstractions.Interfaces;

public interface IAudioQueueItem
{
    public string SourceDisplayName { get; }
    public string DisplayName { get; }
    public CancellationToken CancellationToken { get; }
    public TaskCompletionSource? TaskCompletionSource { get; }
    public event EventHandler<AudioQueueItemStateChangedEventArgs>? StateChanged;

    public Task GetAudioPipeAsync(PipeWriter writer,CancellationToken cancellationToken);
}