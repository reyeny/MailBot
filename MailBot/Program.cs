using MailBot.Models;
using MailBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
builder.Services.AddControllers();


// Регистрируем настройки для почты
try
{
    builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
    builder.Services.AddTransient<MailKit.Net.Imap.ImapClient>(_ => new MailKit.Net.Imap.ImapClient());
    builder.Services.AddHostedService<MailPollingService>();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

// Настройки для телеграм бота
var botToken = builder.Configuration["Telegram:BotToken"];
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken!));

const long chatId = 725591578; 
builder.Services.AddTransient<NotificationService>(sp =>
{
    var botClient = sp.GetRequiredService<ITelegramBotClient>();
    return new NotificationService(botClient, chatId);
});

// Создаём приложение
var app = builder.Build();

// Настраиваем маршруты для контроллеров
app.MapControllers();

app.Run();