using SpotifyAPI.Web;

namespace ConsoleApp;

public class SpotifyApiService(SpotifyClient client)
{
    public async Task<PrivateUser> GetUserProfileAsync()
    {
        try
        {
            var profile = await client.UserProfile.Current();

            return profile;
        }
        catch (APIUnauthorizedException e)
        {
            Console.WriteLine("User Not Authorized Error: " + e.Message);
        }
        catch (APIException  e)
        {
            // Prints: invalid id
            Console.WriteLine(e.Message);
            // Prints: BadRequest
            Console.WriteLine(e.Response?.StatusCode);
        }

        return null;    
    }


    public async Task<List<FullArtist>> GetTopArtistsAsync()
    {
        var request = new UsersTopItemsRequest(TimeRange.MediumTerm)
        {
            Limit = 50
        };
        
        var response = await client.UserProfile.GetTopArtists(request);
        
        var result = response.Items?.ToList() ?? [];
        return result;
    }

    public async Task<List<FullTrack>> GetTopTracksAsync()
    {
        var request = new UsersTopItemsRequest(TimeRange.MediumTerm)
        {
            Limit = 50
        };
        
        var response = await client.UserProfile.GetTopTracks(request);

        var result = response.Items?.ToList() ?? [];
        return result;
    }
    
    public async Task<List<PlayHistoryItem>> GetRecentHistory()
    {
        var req = new PlayerRecentlyPlayedRequest() { Limit = 20 }; //maximum limit is 20
        var response = await client.Player.GetRecentlyPlayed(req);
        
        var result = response.Items?.ToList() ?? [];
        return result;
    }
}