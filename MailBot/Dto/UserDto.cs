namespace MailBot.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    public Guid[] UserMailId { get; set; }
    public long chatId { get; set; }
}