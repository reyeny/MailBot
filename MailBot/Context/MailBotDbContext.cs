using MailBot.Models.User;
using Microsoft.EntityFrameworkCore;

namespace MailBot.Context;

public class MailBotDbContext(DbContextOptions<MailBotDbContext> options) : DbContext(options)
{
    public DbSet<User?> Users { get; set; }
    public DbSet<UserMail> UserMails { get; set; }
}