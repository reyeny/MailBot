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
        ImapClient client,
        NotificationService notificationService)
        : BackgroundService
    {
        private readonly MailSettings _mailSettings = mailSettingsOptions.Value;
        private readonly ConcurrentQueue<MimeMessage> _newMessages = new();
        private int _lastMessageCount;

        public IEnumerable<MimeMessage> GetNewMessages()
        {
            while (_newMessages.TryDequeue(out var message))
                yield return message;
        }

        [Obsolete("Obsolete")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                ConfigureClient();

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

                    if (inbox.Count > _lastMessageCount)
                        await ProcessNewMessagesAsync(inbox, stoppingToken);
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

        private void ConfigureClient()
        {
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.AuthenticationMechanisms.Remove("NTLM");
            client.AuthenticationMechanisms.Remove("GSSAPI");
        }

        [Obsolete("Obsolete")]
        private async Task ProcessNewMessagesAsync(IMailFolder inbox, CancellationToken token)
        {
            for (var i = _lastMessageCount; i < inbox.Count; i++)
            {
                var message = await inbox.GetMessageAsync(i, token);
                _newMessages.Enqueue(message);
                await NotifyAsync(message, token);
            }
            _lastMessageCount = inbox.Count;
        }

        [Obsolete("Obsolete")]
        private async Task NotifyAsync(MimeMessage message, CancellationToken token)
        {
            var subject = message.Subject;
            var body = !string.IsNullOrEmpty(message.TextBody) ? message.TextBody : message.HtmlBody;
            var telegramMessage = $"Новый email:\nТема: {subject}\nСообщение: {body}";
            Console.WriteLine($"telegram: {telegramMessage}");
            await notificationService.NotifyAsync(telegramMessage, token);
        }
    }
}
