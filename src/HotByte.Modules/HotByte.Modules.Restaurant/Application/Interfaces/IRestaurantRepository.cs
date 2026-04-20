namespace HotByte.Modules.Restaurant.Application.Interfaces
{
    public interface IRestaurantRepository
    {
        Task AddAsync(Domain.Entities.Restaurant restaurant);
        Task<List<Domain.Entities.Restaurant>> GetAllAsync();
        Task<Domain.Entities.Restaurant?> GetByIdAsync(int id);
        Task<List<Domain.Entities.Restaurant>> GetByOwnerIdAsync(int ownerUserId);
        Task<List<Domain.Entities.Restaurant>> SearchAsync(string? name, string? category);
        Task UpdateAsync(Domain.Entities.Restaurant restaurant);
        Task DeleteAsync(int id);
    }
}
