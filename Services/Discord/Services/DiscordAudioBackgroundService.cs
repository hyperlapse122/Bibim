using System.IO.Pipelines;
using CliWrap;
using Discord;
using Discord.Audio;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using HyperLapse.Bibim.Service.Discord.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HyperLapse.Bibim.Service.Discord.Services;

internal class DiscordAudioBackgroundService(
    IVoiceChannel channel,
    IAudioQueueService queueService,
    ILogger logger,
    DiscordServiceOptions options
) : BackgroundService
{
    private IAudioClient _audioClient = null!;
    private Stream _audioStream = null!;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _audioClient = await channel.ConnectAsync();

        // Create Audio Stream (macOS is not supported)
        _audioStream = OperatingSystem.IsMacOS()
            ? new MemoryStream()
            : _audioClient.CreatePCMStream(AudioApplication.Music, null, 1000, 20);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (true)
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    var childTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    // If there is no item in the queue, it will wait for 2 minutes.
                    // If there is no item in the queue for 2 minutes, it will cancel the operation and disconnect the audio client.
                    childTokenSource.CancelAfter(TimeSpan.FromMinutes(2));
                    var item = await queueService.DequeueAsync(channel.Id, childTokenSource.Token);

                    var msg = await channel.SendMessageAsync($"Loading `{item.DisplayName}`...");

                    if (OperatingSystem.IsMacOS())
                    {
                        const string t = "macOS is not supported. Skipping playing and waiting 10 seconds.";
                        logger.LogWarning("{message}", t);
                        await channel.SendMessageAsync(t);
                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                        continue;
                    }

                    await channel.ModifyMessageAsync(msg.Id, x => x.Content = $"Playing `{item.DisplayName}`...");

                    var rawPipe = new Pipe();
                    var rawWriter = rawPipe.Writer;
                    var rawReader = rawPipe.Reader;
                    await using var rawPipeReaderStream = rawReader.AsStream();

                    var t1 = item.GetAudioPipeAsync(rawWriter, stoppingToken);

                    var pipe = new Pipe();
                    var writer = pipe.Writer;
                    await using var writeStream = writer.AsStream();

                    var t2 = Cli.Wrap(options.FfmpegPath)
                        .WithArguments(
                            "-hide_banner -i pipe:0 -af loudnorm=I=-36:TP=-2:LRA=7:print_format=json -ac 2 -f s16le -ar 48000 pipe:1"
                        )
                        .WithStandardInputPipe(PipeSource.FromStream(rawPipeReaderStream))
                        .WithStandardOutputPipe(PipeTarget.ToStream(writeStream))
                        .WithStandardErrorPipe(PipeTarget.ToDelegate(e => logger.LogInformation("{message}", e)))
                        .ExecuteAsync(stoppingToken);

                    var reader = pipe.Reader;
                    var t3 = reader.CopyToAsync(_audioStream, stoppingToken);

                    await Task.WhenAll(t1, t2, t3);
                }
                catch (TaskCanceledException)
                {
                    await channel.SendMessageAsync("There was no item in the queue for 2 minutes. Disconnecting...");
                    return;
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "");
            throw;
        }
    }

    public override void Dispose()
    {
        _audioStream.Dispose();
        _audioClient.Dispose();
        base.Dispose();
    }
}