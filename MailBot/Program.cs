using MailBot.Context;
using MailBot.Context.MailController;
using MailBot.Models;
using MailBot.Models.User;
using MailBot.Services;
using MailBot.Services.Telegram_Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MailBotDbContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options => {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<MailBotDbContext>()
    .AddDefaultTokenProviders();



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