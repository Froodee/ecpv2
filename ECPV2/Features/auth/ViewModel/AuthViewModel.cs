using ECPV2.Core.ViewModels;
using ECPV2.Domain.Models;
using ECPV2.Services.Command;
using ECPV2.Services.Smpt;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ECPV2.Features.auth.ViewModel
{
    internal class AuthViewModel : BaseViewModel, IAuthViewModel
    {
        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _pin = string.Empty;
        private string _message = string.Empty;
        private bool _isPinSent = false;

        private string _generatedPin = string.Empty;
        private readonly EcpContext _context;

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Pin
        {
            get => _pin;
            set => SetProperty(ref _pin, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public bool IsPinSent
        {
            get => _isPinSent;
            set => SetProperty(ref _isPinSent, value);
        }

        public ICommand SendPinCommand { get; set; }
        public ICommand ConnectCommand { get; set; }

        public event EventHandler<bool>? AuthenticationCompleted;

        public AuthViewModel()
        {
            try
            {
                EnsureLogDirectory();
                _context = new EcpContext();

                SendPinCommand = new RelayCommand(async _ => await ActionSendPin(), _ => CanSendPin());
                ConnectCommand = new RelayCommand(async _ => await ActionConnect(), _ => CanConnect());
            }
            catch (Exception ex)
            {
                LogException("Erreur initialisation AuthViewModel", ex);
                Message = "Erreur d'initialisation";
            }
        }

        private bool CanSendPin() => !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);

        private async Task ActionSendPin()
        {
            try
            {
                Message = "Vérification des identifiants...";

                // Vérifier si les identifiants existent dans la base
                var connexion = await _context.Connexions
                    .FirstOrDefaultAsync(c => c.Login == Login && c.Password == Password);

                if (connexion == null)
                {
                    Message = "Login ou mot de passe incorrect";
                    LogInfo($"Tentative de connexion échouée pour {Login}");
                    return;
                }

                // Générer un PIN aléatoire à 6 chiffres
                _generatedPin = new Random().Next(100000, 999999).ToString();

                // Envoyer le PIN par email
                Message = "Envoi du code PIN...";
                var smtpService = SmtpService.Instance;
                bool emailSent = await smtpService.Send(Login, _generatedPin);

                if (emailSent)
                {
                    IsPinSent = true;
                    Message = "Code PIN envoyé par email !";
                    LogInfo($"PIN envoyé à {Login}");
                }
                else
                {
                    Message = "Erreur lors de l'envoi de l'email";
                    LogException("Erreur envoi email", new Exception($"Échec envoi à {Login}"));
                }
            }
            catch (Exception ex)
            {
                Message = "Erreur lors de l'envoi du code PIN";
                LogException("Erreur ActionSendPin", ex);
            }
        }

        private bool CanConnect() => IsPinSent && !string.IsNullOrWhiteSpace(Pin);

        private async Task ActionConnect()
        {
            try
            {
                Message = "Vérification du code PIN...";

                // Vérifier le PIN
                if (Pin != _generatedPin)
                {
                    Message = "Code PIN incorrect";
                    LogInfo($"Code PIN incorrect pour {Login}");
                    return;
                }

                // Récupérer l'utilisateur
                var connexion = await _context.Connexions
                    .Include(c => c.Idusers)
                    .FirstOrDefaultAsync(c => c.Login == Login);

                if (connexion == null || !connexion.Idusers.Any())
                {
                    Message = "Utilisateur non trouvé";
                    return;
                }

                Message = "Connexion réussie !";
                LogInfo($"Connexion réussie pour {Login}");

                // Déclencher l'événement de connexion réussie
                AuthenticationCompleted?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                Message = "Erreur lors de la connexion";
                LogException("Erreur ActionConnect", ex);
            }
        }

        public override void Dispose()
        {
            _context?.Dispose();
            base.Dispose();
        }
    }
}
