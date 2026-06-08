using System.Text.Json.Serialization;

namespace TechMove.ApiDtos
{
    public class FirebaseLoginResponse
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("localId")]
        public string LocalId { get; set; } = string.Empty;

        [JsonPropertyName("expiresIn")]
        public string ExpiresIn { get; set; } = string.Empty;
    }
}