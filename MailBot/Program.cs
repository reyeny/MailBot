using MailBot.Context;
using MailBot.Controllers;
using MailBot.Models;
using MailBot.Services;
using MailBot.Services.Telegram_Bot_Services;
using MailBot.Services.Telegram_Bot_Services.User_Services;
using MailBot.Services.Telegram_Bot_Services.User_Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);

// Загрузка конфигураций
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

// Настройка подключения к базе данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MailBotDbContext>(opt => opt.UseNpgsql(connectionString));

// // Настройка почтовых сервисов
// builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
// builder.Services.AddTransient<MailKit.Net.Imap.ImapClient>(_ => new MailKit.Net.Imap.ImapClient());
// builder.Services.AddHostedService<MailPollingService>();

// Регистрируем контроллеры и необходимые сервисы
builder.Services.AddControllers();
builder.Services.AddSingleton<MailBotController>();
builder.Services.AddSingleton<BotService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<UserCommandService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();


// Настройка Telegram-бота
var botToken = builder.Configuration["Telegram:BotToken"];
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken!));

var app = builder.Build();

// Получаем экземпляры зарегистрированных сервисов
var botClient = app.Services.GetRequiredService<ITelegramBotClient>();
var mailBotController = app.Services.GetRequiredService<MailBotController>();

// Создаем ReceiverOptions для получения обновлений
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = [UpdateType.Message]
};

// Создаем CancellationTokenSource
using var cts = new CancellationTokenSource();

// Запускаем получение обновлений, передавая методы с корректными сигнатурами
botClient.StartReceiving(
    mailBotController.UpdateHandler,
    mailBotController.ErrorHandler,
    receiverOptions,
    cts.Token);

Console.WriteLine("Бот запущен. Для завершения работы нажмите любую клавишу...");
Console.ReadKey();
cts.Cancel();

app.MapControllers();
app.Run();
