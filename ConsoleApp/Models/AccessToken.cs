namespace ConsoleApp.Models;

public class AccessToken
{
    public required string access_token { get; set; }
    public required string token_type { get; set; }
    public required string scope { get; set; }
    public int expires_in { get; set; }
    public string? refresh_token { get; set; }
}