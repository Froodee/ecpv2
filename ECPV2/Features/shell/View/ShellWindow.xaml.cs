using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using ECPV2.Features.client.View;
using ECPV2.Features.employe.View;
using ECPV2.Features.admin.View;
using ECPV2.Features.produit.View;
using ECPV2.Features.promo.View;
using ECPV2.Features.avis.View;
using ECPV2.Features.auth.View;


namespace ECPV2.Features.shell.View
{
    
    public sealed partial class ShellWindow : Window
    {
        private const string LOG_PATH = @"C:\Logs\log.txt";
        private Window WindowFrame;

        public ShellWindow()
        {
            try
            {
                EnsureLogDirectory();
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                LogException("Erreur initialisation ShellWindow", ex);
            }
        }

        private void nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            try
            {
                // V�rifier si un �l�ment est s�lectionn�
                if (args.SelectedItemContainer != null)
                {
                    // R�cup�rer le tag de l'�l�ment s�lectionn� (ou une autre propri�t� comme le contenu)
                    var selectedItem = args.SelectedItemContainer as NavigationViewItem;
                    string? tag = selectedItem?.Tag?.ToString();

                    
                    switch (tag)
                    {
                        case "Dashboard":
                            // TODO: Créer une page Dashboard
                            break;

                        case "Client":
                            MainFrame.Navigate(typeof(ClientPage));
                            break;

                        case "Employe":
                            MainFrame.Navigate(typeof(EmployePage));
                            break;

                        case "Admin":
                            MainFrame.Navigate(typeof(AdminPage));
                            break;

                        case "Produit":
                            MainFrame.Navigate(typeof(ProduitPage));
                            break;

                        case "Promo":
                            MainFrame.Navigate(typeof(PromoPage));
                            break;

                        case "Avis":
                            MainFrame.Navigate(typeof(AvisPage));
                            break;

                        case "Statistique":
                            // TODO: Créer une page Statistiques
                            break;

                        case "Logout":
                            HandleLogout();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("Erreur navigation ShellWindow", ex);
            }
        }

        private void HandleLogout()
        {
            try
            {
                // Fermer la fenêtre shell et rouvrir la page de connexion
                var authWindow = new Window();
                authWindow.Content = new AuthPage();
                authWindow.Activate();
                this.Close();
            }
            catch (Exception ex)
            {
                LogException("Erreur déconnexion", ex);
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
