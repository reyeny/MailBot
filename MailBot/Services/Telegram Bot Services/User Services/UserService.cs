using MailBot.Context;
using MailBot.Dto;
using MailBot.Models.User;
using MailBot.Services.Telegram_Bot_Services.User_Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MailBot.Services.Telegram_Bot_Services.User_Services;

public class UserService(MailBotDbContext context) : IUserService
{
    public async Task<User?> CreateUserAsync(User? user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<List<User?>> GetAllUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
        
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }    
    }
}