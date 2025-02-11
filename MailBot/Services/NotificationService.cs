using Telegram.Bot;
using System.Threading;
using System.Threading.Tasks;

namespace MailBot.Services
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