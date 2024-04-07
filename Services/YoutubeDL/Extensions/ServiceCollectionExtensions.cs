using Microsoft.Extensions.DependencyInjection;

namespace HyperLapse.Bibim.Service.YoutubeDL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYoutubeDL(this IServiceCollection services)
    {
        return services;
    }
}