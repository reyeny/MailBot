using MailBot.Services;
using MailBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MailBot.Controllers
{
    public class BotController
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TelegramBotClient _botClient;
        private readonly NotificationService _notificationService;
        private const int ChatId = 725591578;

        public BotController(string token)
        {
            _botClient = new TelegramBotClient(token);
            _telegramBotService = new TelegramBotService(_botClient);
            _cancellationTokenSource = new CancellationTokenSource();
            _notificationService = new NotificationService(_botClient, ChatId );
        }

        [Obsolete("Obsolete")]
        public async Task Start()
        {
            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"{me.FirstName} {me.LastName}");

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                cancellationToken: _cancellationTokenSource.Token);

            await _notificationService.NotifyAsync("Хеллоу", _cancellationTokenSource.Token);
            
        }
        
        private async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message)
            {
                await _telegramBotService.ProcessUpdateAsync(update, cancellationToken);
            }
        }
        
        private static Task HandleErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiEx => $"Telegram API Error:\n[{apiEx.ErrorCode}]\n{apiEx.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
