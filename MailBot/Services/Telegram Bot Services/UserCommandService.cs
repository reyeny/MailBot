using System.Collections.Concurrent;
using MailBot.Services.Telegram_Bot_Services.User_Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MailBot.Services.Telegram_Bot_Services;

public class UserCommandService
{
    private readonly IUserService _userService;
    private readonly IMailService _mailService;
    private static readonly ConcurrentDictionary<long, TaskCompletionSource<Message>> PendingResponses = new();

    public UserCommandService(IUserService userService, IMailService mailService)
    {
        _userService = userService;
        _mailService = mailService;
    }
    
    public async Task UserCommandHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message is null)
            return;
    
        var message = update.Message;
        var chatId = message.Chat.Id;
        Console.WriteLine($"[Handler] Получено сообщение от чата {chatId}: {message.Text}");

        // Если сообщение не начинается с "/" — считаем, что это ответ на ожидание
        if (!string.IsNullOrEmpty(message.Text) && !message.Text.StartsWith("/"))
        {
            if (PendingResponses.TryRemove(chatId, out var pendingTcs))
            {
                Console.WriteLine($"[Handler] Завершаю ожидание для чата {chatId} с сообщением: {message.Text}");
                pendingTcs.TrySetResult(message);
                return;
            }
        }
    
        // Если это команда, обрабатываем её
        if (!string.IsNullOrEmpty(message.Text))
        {
            switch (message.Text)
            {
                case "/registerNewUser":
                    var userId = message.From!.Id;
                    await NewUserRegistration(userId, chatId, botClient);
                    break;
            
                case "/addMail":
                    await AddEmail(botClient, message, cancellationToken);
                    break;
            
                default:
                    Console.WriteLine("[Handler] Неизвестная команда или сообщение");
                    break;
            }
        }
    }



    [Obsolete("Obsolete")]
    private async Task NewUserRegistration(long userId, long chatId, ITelegramBotClient botClient)
    {
        try
        {
            if (await _userService.NeedToLoginAsync(userId))
            {
                await _userService.CreateUserAsync(userId, chatId);
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Пользователь успешно зарегестрирован");
            }
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

    [Obsolete("Obsolete")]
    private async Task AddEmail(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var userId = message.From!.Id;
        var user = await _userService.GetUserByIdAsync(userId);
    
        if (user == null)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Пожалуйста, зарегистрируйтесь, используя команду /registerNewUser.",
                cancellationToken: cancellationToken);
            return;
        }
    
        // Запрашиваем логин
        await botClient.SendTextMessageAsync(
            chatId,
            "Введите ваш логин для почты:",
            cancellationToken: cancellationToken);
    
        var loginResponse = await WaitForUserResponseAsync(chatId, cancellationToken);
        var login = loginResponse.Text;
    
        // Запрашиваем пароль
        await botClient.SendTextMessageAsync(
            chatId,
            "Введите ваш пароль для почты:",
            cancellationToken: cancellationToken);
    
        var passwordResponse = await WaitForUserResponseAsync(chatId, cancellationToken);
        var password = passwordResponse.Text;

        await _mailService.AddUserMailAsync(userId, login!, password!);

        await botClient.SendTextMessageAsync(
            chatId,
            "Ваша почта успешно добавлена!",
            cancellationToken: cancellationToken);
    }
    
    private Task<Message> WaitForUserResponseAsync(long chatId, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<Message>(TaskCreationOptions.RunContinuationsAsynchronously);
        PendingResponses[chatId] = tcs;
    
        cancellationToken.Register(() =>
        {
            Console.WriteLine($"[Wait] Ожидание для чата {chatId} отменено.");
            tcs.TrySetCanceled();
        });
    
        Console.WriteLine($"[Wait] Ожидаю ответ от чата {chatId}...");
        return tcs.Task;
    }

}

