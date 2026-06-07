using TechMove.Models;

namespace TechMove.Api.Repositories
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
    }
}