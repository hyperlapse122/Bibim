using HyperLapse.Bibim.Service.YoutubeDL.Service;
using Microsoft.Extensions.DependencyInjection;

namespace HyperLapse.Bibim.Service.YoutubeDL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYoutubeDL(this IServiceCollection services)
    {
        return services
            .AddSingleton<YouTubeDLAudioQueueService>()
            .AddSingleton(new YoutubeDLSharp.YoutubeDL
            {
                OutputFolder = Path.GetTempPath(),
                OverwriteFiles = true,
                YoutubeDLPath = "yt-dlp",
                FFmpegPath = "ffmpeg"
            });
    }
}