using HyperLapse.Bibim.Service.Abstractions.Enums;

namespace HyperLapse.Bibim.Service.Abstractions.EventArgs;

public class AudioQueueItemStateChangedEventArgs(AudioQueueItemPlayStatus status) : System.EventArgs
{
    public AudioQueueItemPlayStatus Status { get; } = status;
}