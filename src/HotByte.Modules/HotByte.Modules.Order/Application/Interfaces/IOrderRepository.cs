namespace HotByte.Modules.Order.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Domain.Entities.Order order);
        Task<Domain.Entities.Order?> GetByIdAsync(int id);
        Task<List<Domain.Entities.Order>> GetByUserIdAsync(int userId);
        Task<List<Domain.Entities.Order>> GetByRestaurantIdAsync(int restaurantId);
        Task<List<Domain.Entities.Order>> GetByRestaurantIdsAsync(List<int> restaurantIds);
        Task<List<Domain.Entities.Order>> GetAllAsync();
        Task UpdateAsync(Domain.Entities.Order order);
    }
}
