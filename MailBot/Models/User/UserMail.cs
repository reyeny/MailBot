using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MailBot.Models.User;

public class UserMail
{
    public Guid Id { get; set; }
    public string Mail { get; set; }
    public string Password { get; set; }
    
    public long UserId { get; set; }
    public User User { get; set; }
}