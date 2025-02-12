namespace MailBot.Models.User;

public class User
{
    public long Id { get; set; }
    public Guid[]? UserMailId { get; set; }
    public UserMail[]? UserMails { get; set; }
    public long ChatId { get; set; }
}