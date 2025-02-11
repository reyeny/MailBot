using MailBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MailBot.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ITelegramBotClient _botClient;
        private const long AllowedChatId = 725591578;

        public TelegramBotService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        [Obsolete("Obsolete")]
        public async Task ProcessUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null) return;
            var chatId = update.Message.Chat.Id;

            if (chatId != AllowedChatId)
            {
                Console.WriteLine($"Игнорируем сообщение от неразрешённого пользователя. ChatId: {chatId}");
                return;
            }

            var message = update.Message.Text;
            await SendTextMessageAsync(chatId, message!, cancellationToken);
        }

        [Obsolete("Obsolete")]
        private Task SendTextMessageAsync(long chatId, string message, CancellationToken cancellationToken)
        {
            return _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                cancellationToken: cancellationToken);
        }
    }
}