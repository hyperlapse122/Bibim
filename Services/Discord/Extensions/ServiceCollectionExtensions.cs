using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using HyperLapse.Bibim.Service.Discord.Models;
using HyperLapse.Bibim.Service.Discord.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HyperLapse.Bibim.Service.Discord.Extensions;

public static class ServiceCollectionExtensions
{
    public delegate string GetFfmpegPath(IServiceProvider sp);

    public static IServiceCollection AddDiscord(this IServiceCollection services, GetFfmpegPath getFfmpegPath)
    {
        return services
            .AddSingleton(sp => new DiscordServiceOptions
            {
                FfmpegPath = getFfmpegPath(sp)
            })
            .AddSingleton<IDiscordAudioService, DiscordAudioService>()
            .AddHostedService(sp => sp.GetRequiredService<IDiscordAudioService>());
    }
}