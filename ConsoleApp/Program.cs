using ConsoleApp.Ai;
using ConsoleApp.Services;

var keywordAgent = new KeywordExtractionAgent();
var spotifyApi = new SpotifyApiService();

Console.WriteLine("Enter text:");
var input = Console.ReadLine() ?? "";

var keywords = await keywordAgent.ExtractKeywords(input);

Console.WriteLine("\nKeywords:");
Console.WriteLine(keywords);

var tracks = await spotifyApi.SearchTracksAsync(keywords);

Console.WriteLine("\nTracks:");
foreach (var track in tracks)
{
    Console.WriteLine(track);
}