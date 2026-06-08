using TechMove.ApiDtos;

namespace TechMove.ApiClients
{
    public interface IFirebaseAuthClient
    {
        Task<FirebaseLoginResponse?> LoginAsync(string email, string password);
    }
}