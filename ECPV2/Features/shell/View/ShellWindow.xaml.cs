using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ECPV2.Features.client.View;


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
                // Vérifier si un élément est sélectionné
                if (args.SelectedItemContainer != null)
                {
                    // Récupérer le tag de l'élément sélectionné (ou une autre propriété comme le contenu)
                    var selectedItem = args.SelectedItemContainer as NavigationViewItem;
                    string? tag = selectedItem?.Tag?.ToString();

                    
                    switch (tag)
                    {
                        case "Client":
                            
                            MainFrame.Navigate(typeof(ClientPage));
                            break;
                        
                        
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("Erreur navigation ShellWindow", ex);
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
