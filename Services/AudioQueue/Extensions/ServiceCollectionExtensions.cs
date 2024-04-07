using HyperLapse.Bibim.Service.Abstractions.Interfaces;
using HyperLapse.Bibim.Service.AudioQueue.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HyperLapse.Bibim.Service.AudioQueue.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudioQueue(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAudioQueueService, AudioQueueService>();
    }
}