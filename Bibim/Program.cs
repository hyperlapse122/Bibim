using Bibim.Extensions;
using Bibim.Models;
using HyperLapse.Bibim.Service.AudioQueue.Extensions;
using HyperLapse.Bibim.Service.Discord.Extensions;
using HyperLapse.Bibim.Service.YouTube.Extensions;
using HyperLapse.Bibim.Service.YoutubeDL.Extensions;
using Microsoft.Extensions.Options;
using Sentry.Profiling;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry(o =>
{
    o.Dsn = "https://1a6a7814ed5b4f4dee08df28e6f43a58@o4507203739910144.ingest.us.sentry.io/4507203741351936";
    // When configuring for the first time, to see what the SDK is doing:
    o.Debug = true;
    // Set TracesSampleRate to 1.0 to capture 100%
    // of transactions for performance monitoring.
    // We recommend adjusting this value in production
    o.TracesSampleRate = 1.0;
    // Sample rate for profiling, applied on top of othe TracesSampleRate,
    // e.g. 0.2 means we want to profile 20 % of the captured transactions.
    // We recommend adjusting this value in production.
    o.ProfilesSampleRate = 1.0;
    // Requires NuGet package: Sentry.Profiling
    // Note: By default, the profiler is initialized asynchronously. This can
    // be tuned by passing a desired initialization timeout to the constructor.
    o.AddIntegration(
        new ProfilingIntegration(
            // During startup, wait up to 500ms to profile the app startup code.
            // This could make launching the app a bit slower so comment it out if you
            // prefer profiling to start asynchronously.
            TimeSpan.FromMilliseconds(500)
        )
    );
});

// Configure the services
builder.Services.Configure<DiscordOptions>(builder.Configuration.GetSection(DiscordOptions.SectionName));
builder.Services.Configure<CliOptions>(builder.Configuration.GetSection(CliOptions.SectionName));

// Add services to the container.
builder.Services
    .AddYouTube()
    .AddYoutubeDL(
        sp => sp.GetRequiredService<IOptions<CliOptions>>().Value.YoutubeDlPath,
        sp => sp.GetRequiredService<IOptions<CliOptions>>().Value.FfmpegPath
    )
    .AddDiscord(sp => sp.GetRequiredService<IOptions<CliOptions>>().Value.FfmpegPath)
    .AddAudioQueue()
    .AddDiscordClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();