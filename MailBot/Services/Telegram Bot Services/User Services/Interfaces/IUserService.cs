using MailBot.Models.User;

namespace MailBot.Services.Telegram_Bot_Services.User_Services.Interfaces;

public interface IUserService
{
    Task<User?> CreateUserAsync(long userId, long chatId);
    Task<User?> GetUserByIdAsync(long id);
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(long id);
    Task<bool> NeedToLoginAsync(long id);
}