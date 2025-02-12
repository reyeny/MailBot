using MailBot.Models.User;

namespace MailBot.Services.Telegram_Bot_Services.User_Services.Interfaces;

public interface IUserService
{
    Task<User> CreateUserAsync(User user);
    Task<User> GetUserByIdAsync(string id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(string id);
    Task<int> CountUsersAsync();

}