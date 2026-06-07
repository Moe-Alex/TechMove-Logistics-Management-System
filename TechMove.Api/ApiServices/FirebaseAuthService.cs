using FirebaseAdmin.Auth;

namespace TechMove.Api.Services
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        public async Task<string?> VerifyTokenAsync(string idToken)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return decodedToken.Uid;
            }
            catch
            {
                return null;
            }
        }
    }
}