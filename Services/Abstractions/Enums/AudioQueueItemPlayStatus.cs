namespace HyperLapse.Bibim.Service.Abstractions.Enums;

/// <summary>
///     Represents the status of the audio queue item.
/// </summary>
public enum AudioQueueItemPlayStatus
{
    /// <summary>
    ///     Represents the status of the audio queue item when it is enqueued.
    /// </summary>
    Enqueued,

    /// <summary>
    ///     Represents the status of the audio queue item when it is stopped.
    /// </summary>
    Stopped,

    /// <summary>
    ///     Represents the status of the audio queue item when it is playing.
    /// </summary>
    Playing,

    /// <summary>
    ///     Represents the status of the audio queue item when it is paused.
    /// </summary>
    Paused,

    /// <summary>
    ///     Represents the status of the audio queue item when it is completed.
    /// </summary>
    Completed,

    /// <summary>
    ///     Represents the status of the audio queue item when an error occurred.
    /// </summary>
    ErrorOccurred
}