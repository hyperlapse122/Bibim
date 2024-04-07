using HyperLapse.Bibim.Service.YouTube.Services;
using Microsoft.Extensions.DependencyInjection;
using YoutubeExplode;

namespace HyperLapse.Bibim.Service.YouTube.Extensions;

public static class ServiceCollectionExtension
{
    private static HttpClient HttpClient => new()
    {
        DefaultRequestHeaders =
        {
            {
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3"
            }
        }
    };

    public static IServiceCollection AddYouTube(this IServiceCollection services)
    {
        return services
            .AddSingleton<YouTubeAudioQueueService>()
            .AddSingleton(new YoutubeClient(HttpClient));
    }
}