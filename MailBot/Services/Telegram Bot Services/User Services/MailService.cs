using MailBot.Context;
using MailBot.Models.User;
using MailBot.Services.Telegram_Bot_Services.User_Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailBot.Services.Telegram_Bot_Services.User_Services;

public class MailService(MailBotDbContext _context, IUserService _userService) : IMailService
{
    public async Task<bool> AddUserMailAsync(long userId, string mail, string password)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        
        if (user != null)
        {
            var userMail = await FindMailAsync(userId, mail) ?? new UserMail
            {
                Mail = mail,
                Password = password,
                UserId = userId
            };
            _context.UserMails.Add(userMail);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
    
    public async Task<UserMail?> FindMailAsync(long userId, string mail)
    {
        var userMail = await _context.UserMails
            .FirstOrDefaultAsync(userMail 
                => userMail.Mail == mail && userMail.UserId == userId);
        return userMail;
    }
}