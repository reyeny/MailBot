using MailBot.Models;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace MailBot.Services
{
    public class MailPollingService(
        IOptions<MailSettings> mailSettingsOptions,
        ILogger<MailPollingService> logger,
        ImapClient client)
        : BackgroundService
    {
        
        private readonly MailSettings _mailSettings = mailSettingsOptions.Value;
        private readonly ConcurrentQueue<MimeMessage> _newMessages = new();
        private int _lastMessageCount;
        
        public IEnumerable<MimeMessage> GetNewMessages()
        {
            while (_newMessages.TryDequeue(out var message))
            {
                yield return message;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.AuthenticationMechanisms.Remove("NTLM");
                client.AuthenticationMechanisms.Remove("GSSAPI");

                await client.ConnectAsync(_mailSettings.ImapServer, _mailSettings.ImapPort, SecureSocketOptions.SslOnConnect, stoppingToken);
                await client.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password, stoppingToken);

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);
                _lastMessageCount = inbox.Count;

                logger.LogInformation("Подключение выполнено. Запуск опроса почты...");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                    await inbox.CheckAsync(stoppingToken);

                    if (inbox.Count <= _lastMessageCount) continue;
                    for (var i = _lastMessageCount; i < inbox.Count; i++)
                    {
                        var message = await inbox.GetMessageAsync(i, stoppingToken);
                        _newMessages.Enqueue(message);

                        Console.WriteLine($"Тема: {message.Subject}");
                        Console.WriteLine("Сообщение:");
                        Console.WriteLine(!string.IsNullOrEmpty(message.TextBody) ? message.TextBody : message.HtmlBody);
                        Console.WriteLine(new string('-', 50));
                    }
                    _lastMessageCount = inbox.Count;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при опросе почты.");
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true, stoppingToken);
            }
        }
    }
}
