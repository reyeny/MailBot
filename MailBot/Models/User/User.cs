namespace MailBot.Models.User;

public class User
{
    public Guid Id { get; set; }
    public Guid[] UserMailId { get; set; }
    public UserMail[] UserMails { get; set; }
    public long chatId { get; set; }
}