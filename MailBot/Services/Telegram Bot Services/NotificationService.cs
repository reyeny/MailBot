using Telegram.Bot;

namespace MailBot.Services.Telegram_Service
{
    public class NotificationService(ITelegramBotClient botClient, long chatId)
    {
        [Obsolete("Obsolete")]
        public async Task NotifyAsync(string message, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                cancellationToken: cancellationToken);
        }
    }
}