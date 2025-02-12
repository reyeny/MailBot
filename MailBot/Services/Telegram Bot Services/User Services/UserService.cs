using MailBot.Context;
using MailBot.Models.User;
using MailBot.Services.Telegram_Bot_Services.User_Services.Interfaces;

namespace MailBot.Services.Telegram_Bot_Services.User_Services;

public class UserService(MailBotDbContext context) : IUserService
{
    public async Task<User?> CreateUserAsync(long userId, long chatId)
    {
        var user = new User()
        {
            Id = userId,
            ChatId = chatId
        };
        
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByIdAsync(long id) 
        =>  await context.Users.FindAsync(id);
    
    
    public async Task<User> UpdateUserAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUserAsync(long id)
    {
        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }    
    }

    public async Task<bool> NeedToLoginAsync(long id)
    {
        var user = await GetUserByIdAsync(id);
        return user is null;
    }
}