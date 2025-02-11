using Telegram.Bot.Types;

namespace MailBot.Services.Interfaces;

public interface ITelegramBotService
{
    Task ProcessUpdateAsync(Update update, CancellationToken cancellationToken);
}