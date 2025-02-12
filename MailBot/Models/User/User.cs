using Microsoft.AspNetCore.Identity;

namespace MailBot.Models.User;

public class User : IdentityUser
{
    public Guid[] UserMailId { get; set; }
    public UserMail[] UserMails { get; set; }
    public long chatId { get; set; }
}