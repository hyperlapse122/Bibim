using CliWrap;
using Discord;
using Discord.Audio;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HyperLapse.Bibim.Service.Discord.Services;

internal class DiscordAudioBackgroundService(
    IVoiceChannel channel,
    IAudioQueueService queueService,
    ILogger logger
) : BackgroundService
{
    internal bool IsRunning { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            IsRunning = true;
            using var audioClient = await channel.ConnectAsync();

            // Create Audio Stream (macOS is not supported)
            await using Stream audioStream = OperatingSystem.IsMacOS()
                ? new MemoryStream()
                : audioClient.CreatePCMStream(AudioApplication.Music, null, 1000, 20);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var childTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    // If there is no item in the queue, it will wait for 2 minutes.
                    // If there is no item in the queue for 2 minutes, it will cancel the operation and disconnect the audio client.
                    childTokenSource.CancelAfter(TimeSpan.FromMinutes(2));
                    var item = await queueService.DequeueAsync(channel.Id, cancellationToken: childTokenSource.Token);

                    await channel.SendMessageAsync($"{item.DisplayName} will be played");

                    await using var stream = await item.GetAudioStreamAsync(stoppingToken);

                    if (OperatingSystem.IsMacOS())
                    {
                        logger.LogWarning("macOS is not supported. Skipping audio processing and waiting 10 seconds.");
                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                        continue;
                    }

                    await using var bufferStream = new MemoryStream();

                    await Cli.Wrap("ffmpeg")
                        .WithArguments(
                            "-hide_banner -i pipe:0 -af loudnorm=I=-36:TP=-2:LRA=7:print_format=json -ac 2 -f s16le -ar 48000 pipe:1")
                        .WithStandardInputPipe(PipeSource.FromStream(stream))
                        .WithStandardOutputPipe(PipeTarget.ToStream(bufferStream))
                        .WithStandardErrorPipe(PipeTarget.ToDelegate(e => logger.LogInformation("{message}", e)))
                        .ExecuteAsync(cancellationToken: stoppingToken);
                    await bufferStream.FlushAsync(stoppingToken);
                    bufferStream.Seek(0, SeekOrigin.Begin);

                    await bufferStream.CopyToAsync(audioStream, stoppingToken);
                    await audioStream.FlushAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "");
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "");
            throw;
        }
        finally
        {
            IsRunning = false;
        }
    }
}