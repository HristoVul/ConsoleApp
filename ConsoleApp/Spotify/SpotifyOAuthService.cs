using System.Diagnostics;
using System.Net;
using ConsoleApp.Models;
using SpotifyAPI.Web;

namespace ConsoleApp;

public class SpotifyOAuthService
{
    private const string ClientId = "3891c55b0f524dcda646236777f16894";
    private const string RedirectUri = "http://127.0.0.1:5000/callback";

    public AccessToken CurrentAccess { get; private set; }

    public async Task<SpotifyClient> AuthenticateAsync()
    {
        var (verifier, challenge) = PKCEUtil.GenerateCodes();

        var loginRequest = new LoginRequest(new Uri(RedirectUri), ClientId, LoginRequest.ResponseType.Code)
        {
            CodeChallengeMethod = "S256",
            CodeChallenge = challenge,
            Scope = [
                Scopes.PlaylistReadPrivate,
                Scopes.UserReadEmail,
                Scopes.UserReadPrivate,
                Scopes.UserTopRead,
                Scopes.UserLibraryRead,
                Scopes.UserReadRecentlyPlayed
            ]
        };
        
        var uri = loginRequest.ToUri();

        Process.Start(new ProcessStartInfo
        {
            FileName = uri.ToString(),
            UseShellExecute = true
        });

        var listener = new HttpListener();
        listener.Prefixes.Add("http://127.0.0.1:5000/callback/");
        listener.Start();


        var context = await listener.GetContextAsync();
        string code = context.Request.QueryString.Get("code");

        listener.Stop();

        return await GetCallback(code, verifier);
    }

    public async Task<SpotifyClient> FromAccessToken(AccessToken token)
    {
        try
        {
            var newResponse = await new OAuthClient().RequestToken(
                new PKCETokenRefreshRequest(ClientId, token.RefreshToken)
            );
            
            return CreateClient(newResponse);
        }
        catch (APIException ex) when (ex.Message == "invalid_grant")
        {
            return await AuthenticateAsync();
        }
    }
    
    private async Task<SpotifyClient> GetCallback(string code, string verifier)
    {
        PKCETokenResponse initialResponse = await new OAuthClient().RequestToken(
            new PKCETokenRequest(ClientId, code, new Uri(RedirectUri), verifier)
        );

        return CreateClient(initialResponse);
    }
    
    private SpotifyClient CreateClient(PKCETokenResponse response)
    {
        var authenticator = new PKCEAuthenticator(ClientId, response);

        var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);

        var spotify = new SpotifyClient(config);

        CurrentAccess = response;
        return spotify;
    }
}