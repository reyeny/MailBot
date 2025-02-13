using MailBot.Models.User;

namespace MailBot.Services.Telegram_Bot_Services.User_Services.Interfaces;

public interface IMailService
{
    Task<bool> AddUserMailAsync(long userId, string mail, string password); 
    Task<UserMail?> FindMailAsync(long userId, string mail);
}