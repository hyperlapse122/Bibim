using Bibim.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace Bibim.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscordClient(this IServiceCollection services)
    {
        services
            .AddTransient(sp => sp)
            .AddSingleton<DiscordSocketConfig>()
            .AddSingleton<DiscordSocketClient>()
            .AddTransient(sp => sp.GetRequiredService<DiscordSocketClient>().Rest)
            .AddSingleton<InteractionService>();
        services.AddHostedService<DiscordHostedService>();
        return services;
    }
}