using Telegram.Bot;

namespace MailBot.Services;

public class NotificationService(ITelegramBotClient botClient, long chatId)
{
    
    [Obsolete("Obsolete")]
    public async Task NotifyAsync(string notificationText, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: notificationText,
            cancellationToken: cancellationToken);
    }
}