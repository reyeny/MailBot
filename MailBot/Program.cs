using MailBot.Controllers;
using MailBot.Models;
using MailBot.Services;
using MailBot.Services.Interfaces;
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
var botRunner = new BotController(botToken!);
await botRunner.Start();



// Создаём приложение
var app = builder.Build();

// Настраиваем маршруты для контроллеров
app.MapControllers();

app.Run();