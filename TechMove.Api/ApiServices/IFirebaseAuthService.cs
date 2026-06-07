namespace TechMove.Api.Services
{
    public interface IFirebaseAuthService
    {
        Task<string?> VerifyTokenAsync(string idToken);
    }
}