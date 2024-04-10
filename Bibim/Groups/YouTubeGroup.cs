using Discord;
using Discord.Interactions;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using HyperLapse.Bibim.Service.YouTube.Services;

namespace Bibim.Groups;

[Group("youtube", "YouTube")]
public class YouTubeGroup(
    ILogger<AudioModule> logger,
    IDiscordAudioService audioService,
    YouTubeAudioQueueService youTubeAudioQueueService
)
    : InteractionModuleBase<SocketInteractionContext>
{
    // The command's Run Mode MUST be set to RunMode.Async, otherwise, being connected to a voice channel will block the gateway thread.
    [SlashCommand("yt", "Play YouTube audio", runMode: RunMode.Async, ignoreGroupNames: true)]
    public async Task EnqueueLink(string link)
    {
        if ((Context.User as IGuildUser)?.VoiceChannel is not { } channel)
        {
            await RespondAsync(
                "User must be in a voice channel, or a voice channel must be passed as an argument."
            );
            return;
        }

        try
        {
            var item = await youTubeAudioQueueService.Enqueue(channel.Id, link);

            audioService.EnsureAudioServiceCreated(channel);
            await RespondAsync(
                $"YouTube Video `{item.DisplayName}` has been added to the queue. It will be played soon."
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while trying to play YouTube audio.");
#if DEBUG
            await RespondAsync($"An error occurred: {e.Message}");
#else
            await RespondAsync("An error occurred while trying to play YouTube audio.");
#endif
        }
    }
}