using Bibim.Extensions;
using Bibim.Models;
using HyperLapse.Bibim.Service.AudioQueue.Extensions;
using HyperLapse.Bibim.Service.Discord.Extensions;
using HyperLapse.Bibim.Service.YouTube.Extensions;
using HyperLapse.Bibim.Service.YoutubeDL.Extensions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

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