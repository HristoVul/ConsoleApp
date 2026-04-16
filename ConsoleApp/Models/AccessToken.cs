using SpotifyAPI.Web;

namespace ConsoleApp.Models;

public class AccessToken
{
    public required string Token { get; set; }
    public required string TokenType { get; set; }
    public required string Scope { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = null!;

    public static implicit operator AccessToken(PKCETokenResponse response)
    {
        return new AccessToken
        {
            Token = response.AccessToken,
            TokenType = response.TokenType,
            Scope = response.Scope,
            ExpiresIn = response.ExpiresIn,
            RefreshToken = response.RefreshToken
        };
    }
}