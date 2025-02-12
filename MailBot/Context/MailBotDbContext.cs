using MailBot.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using User = MailBot.Models.User.User;

namespace MailBot.Context.MailController;

public class MailBotDbContext(DbContextOptions<MailBotDbContext> options)
    : IdentityDbContext<User>(options)
{
    public DbSet<UserMail> UserMails { get; set; }
}