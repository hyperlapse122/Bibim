using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using HyperLapse.Bibim.Service.YoutubeDL.Models;
using HyperLapse.Bibim.Service.YoutubeDL.Service;
using Microsoft.Extensions.DependencyInjection;

namespace HyperLapse.Bibim.Service.YoutubeDL.Extensions;

public static class ServiceCollectionExtensions
{
    public delegate string GetFfmpegPath(IServiceProvider sp);

    public delegate string GetYoutubeDLPath(IServiceProvider sp);

    public static IServiceCollection AddYoutubeDL(this IServiceCollection services, GetYoutubeDLPath getYoutubeDlPath,
        GetFfmpegPath getFfmpegPath)
    {
        return services
            .AddSingleton<YouTubeDLOptions>(sp => new YouTubeDLOptions
            {
                YoutubeDLPath = getYoutubeDlPath(sp),
                FfmpegPath = getFfmpegPath(sp)
            })
            .AddSingleton<IYouTubeDLAudioQueueService, YouTubeDLAudioQueueService>()
            .AddSingleton(sp => new YoutubeDLSharp.YoutubeDL
            {
                OutputFolder = Path.GetTempPath(),
                OverwriteFiles = true,
                YoutubeDLPath = sp.GetRequiredService<YouTubeDLOptions>().YoutubeDLPath,
                FFmpegPath = sp.GetRequiredService<YouTubeDLOptions>().FfmpegPath
            });
    }
}