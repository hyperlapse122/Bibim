using System.Text.RegularExpressions;
using CliWrap;
using Discord;
using Discord.Audio;
using Discord.Interactions;
using YoutubeDLSharp;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Bibim.Services;

// You can put commands in groups
[Group("group-name", "Group description")]
public partial class CommandGroupModule(ILogger<CommandGroupModule> logger)
    : InteractionModuleBase<SocketInteractionContext>
{
    // This command will look like
    // group-name ping
    [SlashCommand("ping", "Get a pong", true)]
    public async Task PongSubcommand()
    {
        await RespondAsync("Pong!");
    }

    // The command's Run Mode MUST be set to RunMode.Async, otherwise, being connected to a voice channel will block the gateway thread.
    [SlashCommand("yt", "Play YouTube audio", runMode: RunMode.Async, ignoreGroupNames: true)]
    public async Task YouTube(string url)
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            if (!YouTubeLinkRegex().IsMatch(url))
            {
                await RespondAsync("Invalid URL");
                return;
            }

            var dict = YouTubeLinkRegex().Match(url);
            var id = dict.Groups["id"].Value;

            // Get the audio channel
            var channel = (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await RespondAsync(
                    "User must be in a voice channel, or a voice channel must be passed as an argument."
                );
                return;
            }

            await RespondAsync($"value: {id}");

            var youtube = new YoutubeClient(new HttpClient
            {
                DefaultRequestHeaders =
                {
                    {
                        "User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3"
                    }
                }
            });

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(id);
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            using var audioClient = await channel.ConnectAsync();

            await using var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

            await using var tempStream = new MemoryStream();

            var t1 = await Cli.Wrap("ffmpeg")
                .WithArguments("-hide_banner -i pipe:0 -af loudnorm=I=-36:TP=-2:LRA=7:print_format=json -ac 2 -f s16le -ar 48000 pipe:1")
                .WithStandardInputPipe(PipeSource.FromStream(stream))
                .WithStandardOutputPipe(PipeTarget.ToStream(tempStream))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(e => logger.LogInformation("{message}", e)))
                .ExecuteAsync();
            await tempStream.FlushAsync();
            tempStream.Seek(0, SeekOrigin.Begin);

            // var t2 = await Cli.Wrap("opusenc")
            //     .WithArguments("""
            //                     --bitrate 384 --raw "-" "-"
            //                    """)
            //     .WithStandardInputPipe(PipeSource.FromStream(tempStream))
            //     .WithStandardOutputPipe(PipeTarget.ToStream(tempStream2))
            //     .WithStandardErrorPipe(PipeTarget.ToDelegate(e => logger.LogInformation("{message}", e)))
            //     .ExecuteAsync();
            // await tempStream2.FlushAsync();
            // tempStream2.Seek(0, SeekOrigin.Begin);

            await using var discord = audioClient.CreatePCMStream(AudioApplication.Music, null, 1000, 20);
            await tempStream.CopyToAsync(discord);
            await discord.FlushAsync();

            // await Task.WhenAll(t1, t2/*, t3*/);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, null);
            await RespondAsync(ex.Message);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [GeneratedRegex(
        @"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube(-nocookie)?\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|live\/|v\/)?)(?<id>[\w\-]+)(\S+)?$")]
    private static partial Regex YouTubeLinkRegex();


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