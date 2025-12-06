using System;
using System.Security.Cryptography;
using System.Text;


namespace SpotifyAIRecommender.Utils
{
    public static class PkceUtil
    {
        public static (string verifier, string challenge) GeneratePkce()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);


            string verifier = Base64UrlEncode(random);
            using var sha256 = SHA256.Create();
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
            string challenge = Base64UrlEncode(challengeBytes);


            return (verifier, challenge);
        }


        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
