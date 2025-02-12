using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MailBot.Services.Telegram_Services;

public class UserService
{
    
    
    public void UserCommandHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        var chat = message.Chat;
        switch (update.Type)
        {
            case UpdateType.Message:
            {
                switch (message.Text)
                {
                    case "/registerNewUser":
                        var newUser = message.From;
                        long userId = newUser.Id;
                        NewUserRegistration(userId, chat.Id, botClient);
                        return;
                        
                }
                
                return;
            }
        }
}

    private void NewUserRegistration(long userId, long chatId, ITelegramBotClient botClient)
    {
        throw new NotImplementedException();
    }
}