using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ECPV2.Services.Smpt
{
    public class SmtpService
    {
        private const string LOG_PATH = @"C:\Logs\log.txt";
        private readonly IConfiguration configuration;
        private readonly string? _compte;
        private readonly string? _cleSecrete;
        static SmtpService? _instance;

        public static SmtpService Instance
        {
            get
            {
                return _instance ??= new SmtpService();
            }
        }

        private SmtpService()
        {
            try
            {
                EnsureLogDirectory();

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddUserSecrets<MainWindow>();

                configuration = builder.Build();
                _compte = configuration["Authentification:Google:Email"];
                _cleSecrete = configuration["Authentification:Google:CleSecrete"];

                if (string.IsNullOrEmpty(_compte) || string.IsNullOrEmpty(_cleSecrete))
                {
                    LogException("Erreur configuration SMTP", new InvalidOperationException("Email ou clé secrète manquants dans appsettings.json ou UserSecrets"));
                }
            }
            catch (Exception ex)
            {
                LogException("Erreur initialisation SmtpService", ex);
            }
        }

        public async Task<bool> Send(string email, string pin)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
                {
                    Trace.WriteLine($"Email invalide : {email}");
                    LogException("Validation email", new ArgumentException($"Email invalide : {email}"));
                    return false;
                }

                Trace.WriteLine($"Envoi du mail à {email} en cours...");
                Trace.WriteLine($"PIN à envoyer : {pin}");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("CabinetMartin", _compte));
                message.To.Add(new MailboxAddress("To", email));
                message.Subject = "Votre code Pin";
                message.Body = new TextPart("plain") { Text = $"Veuillez saisir votre code pin :\n {pin}" };

                using var client = new MailKit.Net.Smtp.SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_compte, _cleSecrete);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                Trace.WriteLine($"Code pin envoyé à {email} avec succès.");
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Problème lors de l'envoi à {email} : {ex.Message}");
                LogException($"Erreur envoi email à {email}", ex);
                return false;
            }
        }

        // LOGS
        private void EnsureLogDirectory()
        {
            try
            {
                var dir = Path.GetDirectoryName(LOG_PATH);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);
            }
            catch { }
        }

        private void LogException(string context, Exception ex)
        {
            try
            {
                string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}\n" +
                               $"Message: {ex.Message}\n" +
                               $"StackTrace: {ex.StackTrace}\n" +
                               new string('-', 50) + "\n";
                File.AppendAllText(LOG_PATH, entry);
            }
            catch { }
        }
    }
}