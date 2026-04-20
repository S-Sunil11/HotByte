namespace HotByte.SharedKernel.Interfaces
{
    public interface IUserPublicService
    {
        Task<int> CreateRestaurantOwnerAccountAsync(string name, string email, string password, string? phone);
        Task<string> GetUserEmailAsync(int userId);
        Task<string> GetUserNameAsync(int userId);
    }
}
