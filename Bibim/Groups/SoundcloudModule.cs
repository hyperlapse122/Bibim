using Discord;
using Discord.Interactions;
using HyperLapse.Bibim.Service.Discord.Services;
using HyperLapse.Bibim.Service.YoutubeDL.Service;

namespace Bibim.Groups;

// You can put commands in groups
[Group("soundcloud", "Soundcloud")]
public class SoundcloudModule(
    ILogger<SoundcloudModule> logger,
    DiscordAudioService audioService,
    YouTubeDLAudioQueueService youTubeDlAudioQueueService
)
    : InteractionModuleBase<SocketInteractionContext>
{
    // The command's Run Mode MUST be set to RunMode.Async, otherwise, being connected to a voice channel will block the gateway thread.
    [SlashCommand("sc", "Play Soundcloud audio", runMode: RunMode.Async, ignoreGroupNames: true)]
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
            var item = await youTubeDlAudioQueueService.EnqueueAsync(channel.Id, link);

            audioService.EnsureAudioServiceCreated(channel);
            await RespondAsync(
                $"Soundcloud Audio `{item.DisplayName}` has been added to the queue. It will be played soon."
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