using Discord;
using Microsoft.Extensions.Hosting;

namespace HyperLapse.Bibim.Service.Abstractions.Interfaces;

public interface IDiscordAudioService : IHostedService
{
    public void EnsureAudioServiceCreated(IVoiceChannel channel, CancellationToken cancellationToken = default);
}