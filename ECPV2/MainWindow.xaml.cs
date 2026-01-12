using ECPV2.Features.shell.View;
using ECPV2.Features.auth.View;
using Microsoft.UI.Xaml;

namespace ECPV2
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Créer et afficher la page d'authentification
            var authPage = new AuthPage();

            // S'abonner à l'événement de connexion réussie
            authPage.ViewModel.AuthenticationCompleted += (sender, success) =>
            {
                if (success)
                {
                    // Ouvrir le Shell après authentification réussie
                    var shellWindow = new ShellWindow();
                    shellWindow.Activate();
                    this.Close();
                }
            };

            // Afficher la page d'authentification dans cette fenêtre
            this.Content = authPage;
        }
    }
}
