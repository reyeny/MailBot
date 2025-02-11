namespace MailBot.Models;

public class MailSettings
{
    public string ImapServer { get; set; }
    public int ImapPort { get; set; }
    public bool UseSsl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}