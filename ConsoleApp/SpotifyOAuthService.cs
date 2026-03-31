using SpotifyAIRecommender.Utils;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using ConsoleApp.Credentials;
using ConsoleApp.Models;


namespace SpotifyAIRecommender.Services
{
    public class SpotifyOAuthService
    {
        private const string ClientId = "10e4b7afefdd45bfa4ee4a9d5b63cdcf";
        private const string RedirectUri = "http://127.0.0.1:5000/callback";


        public async Task AuthenticateAsync()
        {
            var (verifier, challenge) = PkceUtil.GeneratePkce();


            string authUrl = $"https://accounts.spotify.com/authorize?client_id={ClientId}" +
                             "&response_type=code" +
                             $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                             "&code_challenge_method=S256" +
                             $"&code_challenge={challenge}" +
                             "&scope=user-read-private%20user-read-email%20playlist-read-private";


            Console.WriteLine("Open the following URL:");
            Console.WriteLine(authUrl);


            Process.Start(new ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });


            var listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:5000/callback/");
            listener.Start();


            var context = await listener.GetContextAsync();
            string code = context.Request.QueryString.Get("code");


            byte[] responseBytes = Encoding.UTF8.GetBytes("Auth complete! You can close this window.");
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.Close();


            listener.Stop();


            await ExchangeCodeForToken(code, verifier);
        }


        private async Task ExchangeCodeForToken(string code, string verifier)
        {
            using var client = new HttpClient();


            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", RedirectUri },
                { "code_verifier", verifier }
            });


            var response = await client.PostAsync("https://accounts.spotify.com/api/token", body);
            string json = await response.Content.ReadAsStringAsync();


            var tokenObj = JsonSerializer.Deserialize<AccessToken>(json);

            await CredentialStorage.StoreAsync<AccessToken>("spotify", tokenObj); ;
        }
    }
}