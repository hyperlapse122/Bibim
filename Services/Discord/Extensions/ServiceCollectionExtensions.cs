using HyperLapse.Bibim.Service.Discord.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HyperLapse.Bibim.Service.Discord.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscord(this IServiceCollection services)
    {
        return services
            .AddSingleton<DiscordAudioService>()
            .AddHostedService(sp => sp.GetRequiredService<DiscordAudioService>());
    }
}