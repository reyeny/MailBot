using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MailBot.Services.Telegram_Bot_Services;

public class BotService
{
    public BotService()
    {
        
    }
    
    [Obsolete("Obsolete")]
    public void Command(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        var chat = message!.Chat;

        switch (update.Type)
        {
            case UpdateType.Message:
            {
                switch (message.Text)
                {
                    case "/start":
                        SendWelcomeMessage(chat.Id, botClient);
                        return;
                    default:
                        return;
                }
            }
        }
}

    [Obsolete("Obsolete")]
    private static async Task SendWelcomeMessage(long chatId, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(
            chatId,
            "Welcome to Telegram Bot!"
        );
    }
}