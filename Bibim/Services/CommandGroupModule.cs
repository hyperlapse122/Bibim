using Discord;
using Discord.Interactions;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using HyperLapse.Bibim.Service.Discord.Services;
using HyperLapse.Bibim.Service.YouTube.Services;
using HyperLapse.Bibim.Service.YoutubeDL.Service;

namespace Bibim.Services;

// You can put commands in groups
[Group("group-name", "Group description")]
public class CommandGroupModule(IAudioQueueService queueService)
    : InteractionModuleBase<SocketInteractionContext>
{
    // This command will look like
    // group-name ping
    [SlashCommand("ping", "Get a pong", true)]
    public async Task PongSubcommand()
    {
        await RespondAsync("Pong!");
    }


    [SlashCommand("nextup", "Show the next up audio", runMode: RunMode.Async, ignoreGroupNames: true)]
    public async Task NextUp()
    {
        var channel = (Context.User as IGuildUser)?.VoiceChannel;
        if (channel == null)
        {
            await RespondAsync(
                "User must be in a voice channel, or a voice channel must be passed as an argument."
            );
            return;
        }

        var nextUp = await queueService.PeekAsync(channel.Id);
        if (nextUp == null)
        {
            await RespondAsync("There is no audio in the queue.");
            return;
        }

        await RespondAsync($"Next up: {nextUp.DisplayName}");
    }


    // And even in sub-command groups
    [Group("subcommand-group-name", "Subcommand group description")]
    public class SubCommandGroupModule : InteractionModuleBase<SocketInteractionContext>
    {
        // This command will look like
        // group-name subcommand-group-name echo
        [SlashCommand("echo", "Echo an input")]
        public async Task EchoSubcommand(string input)
        {
            await RespondAsync(input,
                components: new ComponentBuilder().WithButton("Echo", $"echoButton_{input}").Build());
        }

        // Component interaction with ignoreGroupNames set to true
        [ComponentInteraction("echoButton_*", true)]
        public async Task EchoButton(string input)
        {
            await RespondAsync(input);
        }
    }
}