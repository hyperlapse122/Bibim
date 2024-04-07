using Discord;
using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HyperLapse.Bibim.Service.Discord.Services;

public class DiscordAudioService(ILogger<DiscordAudioService> logger, IAudioQueueService queueService) : IHostedService
{
    private readonly Dictionary<ulong, DiscordAudioBackgroundService> _backgroundServices = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(_backgroundServices.Values.Select(e => e.StopAsync(cancellationToken)));
    }

    private DiscordAudioBackgroundService GetService(IVoiceChannel channel,
        CancellationToken cancellationToken = default)
    {
        if (_backgroundServices.TryGetValue(channel.Id, out var service) && service.IsRunning) return service;

        service = new DiscordAudioBackgroundService(channel, queueService, logger);
        service.StartAsync(cancellationToken);
        _backgroundServices[channel.Id] = service;

        return service;
    }

    public void EnsureAudioServiceCreated(IVoiceChannel channel, CancellationToken cancellationToken = default)
    {
        GetService(channel, cancellationToken);
    }
}