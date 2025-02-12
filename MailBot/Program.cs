using MailBot.Context;
using MailBot.Models;
using MailBot.Services;
using MailBot.Services.Telegram_Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Загрузка конфигураций
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddControllers();

// Настройка подключения к базе данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MailBotDbContext>(opt => opt.UseNpgsql(connectionString));

// Настройка почтовых сервисов
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

// Настройка Telegram-бота
var botToken = builder.Configuration["Telegram:BotToken"];
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken!));

const long chatId = 725591578;
builder.Services.AddTransient<NotificationService>(sp =>
{
    var botClient = sp.GetRequiredService<ITelegramBotClient>();
    return new NotificationService(botClient, chatId);
});

// Создаём и настраиваем приложение
var app = builder.Build();

app.MapControllers();

app.Run();