namespace TechMove.ApiDtos
{
    public class FirebaseLoginRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool ReturnSecureToken { get; set; } = true;
    }
}