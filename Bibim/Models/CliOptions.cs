namespace Bibim.Models;

public class CliOptions
{
    public const string SectionName = "Cli";

    public required string FfmpegPath { get; set; }
    public required string YoutubeDlPath { get; set; }
}