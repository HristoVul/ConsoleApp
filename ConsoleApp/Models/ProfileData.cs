namespace ConsoleApp.Models;

public record TrackEntry(string Name, string Artist, string Album, string? TrackHref);

public class ProfileData
{
    public string[] Artists { get; set; } = Array.Empty<string>();
    public string[] Genres { get; set; } = Array.Empty<string>();
    public List<TrackEntry> Tracks { get; set; } = [];
    public List<TrackEntry> PlaybackHistory { get; set; } = [];

    public string ToKeywordReference()
    {
        var artists = Artists.Length > 0
            ? string.Join(", ", Artists)
            : "None";

        var genres = Genres.Length > 0
            ? string.Join(", ", Genres)
            : "None";
        
        var tracks = Tracks.Count > 0
            ? string.Join("; ", Tracks.Select(t => $"{t.Name} — {t.Artist} ({t.Album})"))
            : "None";

        var history = PlaybackHistory.Count > 0
            ? string.Join("; ", PlaybackHistory.Select(t => $"{t.Name} — {t.Artist} ({t.Album})"))
            : "None";

        return $@"
Preferred artists:
{artists}

Preferred genres:
{genres}

Top tracks:
{tracks}

Recent playback history:
{history}
".Trim();
    }
}
