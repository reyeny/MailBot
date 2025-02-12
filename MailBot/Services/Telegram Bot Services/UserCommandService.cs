using MailBot.Services.Telegram_Bot_Services.User_Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MailBot.Services.Telegram_Bot_Services;

public class UserCommandService
{
    private readonly IUserService _userService;
    
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
                        var userId = newUser!.Id;
                        NewUserRegistration(userId, chat.Id, botClient);
                        return;
                }
                return;
            }
        }
}

    [Obsolete("Obsolete")]
    private async void NewUserRegistration(long userId, long chatId, ITelegramBotClient botClient)
    {
        try
        {
            if (await _userService.NeedToLoginAsync(userId)) 
                await _userService.CreateUserAsync(userId, chatId);
            else
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Данный пользователь уже зарегестрирован!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}