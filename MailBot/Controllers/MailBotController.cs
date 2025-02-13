using MailBot.Services.Telegram_Bot_Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace MailBot.Controllers;

public class MailBotController(BotService botService, UserCommandService userCommandService)
{
    public async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            botService.Command(botClient, update, cancellationToken);
            await userCommandService.UserCommandHandler(botClient, update, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    
    public Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var errorMessage = error switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };
    
        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}