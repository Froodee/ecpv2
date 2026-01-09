using ECPV2.Domain.Models;
using ECPV2.Services.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ECPV2.Features.client.ViewModel
{
    internal class ClientViewModel :  INotifyPropertyChanged, IClientViewModel
    {
        private const string LOG_PATH = @"C:\Logs\log.txt";

        private string _nomSearch = string.Empty;
        private Client? _clientSelected;
        private Commande _commandeSelected = new();
        private bool _editable = false;

        private List<Client> _clients = new();
        private ObservableCollection<Commande> _commandes = new();

        public string NomSearch
        {
            get => _nomSearch;
            set { _nomSearch = value; OnPropertyChanged(); }
        }

        public List<Client> Clients
        {
            get => _clients;
            set { _clients = value; OnPropertyChanged(); }
        }

        public Client ClientSelected
        {
            get => _clientSelected ?? new Client();
            set
            {
                _clientSelected = value;
                OnPropertyChanged();
                ReloadCommandes();
            }
        }

        public ObservableCollection<Commande> Commandes
        {
            get => _commandes;
        }

        public Commande CommandeSelected
        {
            get => _commandeSelected;
            set { _commandeSelected = value; OnPropertyChanged(); }
        }

        public bool IsEditable
        {
            get => _editable;
            set { _editable = value; OnPropertyChanged(); }
        }

        
        public ICommand CommandClientNew { get; set; }
        public ICommand CommandClientEdit { get; set; }
        public ICommand CommandClientSave { get; set; }
        public ICommand CommandClientDelete { get; set; }
        public ICommand CommandClientSearch { get; set; }
        public ICommand CommandClientCancel { get; set; }

        private readonly EcpContext _context; 

        public ClientViewModel()
        {
            try
            {
                EnsureLogDirectory();
                _context = new EcpContext();

                CommandClientNew = new RelayCommand(_ => ActionClientNew(), _ => CanClientNew());
                CommandClientEdit = new RelayCommand(_ => ActionClientEdit(), _ => CanClientEdit());
                CommandClientSave = new RelayCommand(_ => ActionClientSave(), _ => CanClientSave());
                CommandClientDelete = new RelayCommand(_ => ActionClientDelete(), _ => CanClientDelete());
                CommandClientSearch = new RelayCommand(_ => ActionClientSearch(), _ => CanClientSearch());
                CommandClientCancel = new RelayCommand(_ => ActionClientCancel(), _ => CanClientCancel());

                Clients = _context.Clients
                    .Include(c => c.IduserNavigation)
                    .ToList();
            }
            catch (Exception ex)
            {
                LogException("Erreur initialisation ClientViewModel", ex);
                Clients = new();
            }
        }

        public void ActionClientNew()
        {
            try
            {
                var utilisateur = new Utilisateur { Nomuser = "", Adruser = "", Cpuser = 0, Villeuser = "", Numuser = 0 };
                ClientSelected = new Client
                {
                    IduserNavigation = utilisateur
                };
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionClientNew", ex);
            }
        }

        public bool CanClientNew() => true;

        public void ActionClientEdit()
        {
            try
            {
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionClientEdit", ex);
            }
        }

        private bool CanClientEdit() => true;

        private bool CanClientSave() => true;

        public void ActionClientSave()
        {
            try
            {
                
                if (ClientSelected.Iduser == 0)
                {
                    var utilisateur = new Utilisateur
                    {
                        Nomuser = ClientSelected.Nomuser,
                        Adruser = ClientSelected.Adruser,
                        Cpuser = ClientSelected.Cpuser,
                        Villeuser = ClientSelected.Villeuser,
                        Numuser = ClientSelected.Numuser
                    };

                    var client = new Client
                    {
                        Siret = ClientSelected.Siret,
                        IduserNavigation = utilisateur
                    };

                    _context.Utilisateurs.Add(utilisateur);
                    _context.SaveChanges(); // Pour générer l'Iduser

                    client.Iduser = utilisateur.Iduser;
                    _context.Clients.Add(client);
                }
                else
                {
                    // Mise à jour
                    _context.Utilisateurs.Update(ClientSelected.IduserNavigation);
                    _context.Clients.Update(ClientSelected);
                }

                _context.SaveChanges();
                IsEditable = false;
                RefreshClientsList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionClientSave", ex);
            }
        }

        public void ActionClientDelete()
        {
            try
            {
                if (ClientSelected!= null)
                {
                    var utilisateur = ClientSelected.IduserNavigation;
                    _context.Clients.Remove(ClientSelected);
                    if (utilisateur != null)
                        _context.Utilisateurs.Remove(utilisateur);
                    _context?.SaveChanges();
                }
                RefreshClientsList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionClientDelete", ex);
            }
        }

        private bool CanClientDelete() => ClientSelected!= null && !IsEditable;

        private bool CanClientSearch() => true;

        public void ActionClientSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NomSearch))
                {
                    RefreshClientsList();
                }
                else
                {
                    var search = NomSearch.ToLower();
                    Clients = _context.Clients
                        .Include(c => c.IduserNavigation)
                        .Where(c => c.Nomuser.ToLower().Contains(search) ||
                                   (c.Siret != null && c.Siret.ToString().Contains(search)) ||
                                   c.IduserNavigation.Villeuser.ToLower().Contains(search))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionClientSearch", ex);
                Clients = new();
            }
        }

        private bool CanClientCancel() => IsEditable;

        public void ActionClientCancel()
        {
            try
            {
                if (ClientSelected != null)
                {
                    _context.Entry(ClientSelected).Reload();
                    if (ClientSelected.IduserNavigation != null)
                        _context.Entry(ClientSelected.IduserNavigation).Reload();
                }
                IsEditable = false;
                ReloadCommandes();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionClientCancel", ex);
            }
        }

        private void ReloadCommandes()
        {
            try
            {
                Commandes.Clear();
                if (ClientSelected == null || ClientSelected.Iduser == 0) return;

                var commandes = _context.Commandes
                    .Where(c => c.Iduser == ClientSelected.Iduser)
                    .ToList();

                foreach (var cmd in commandes)
                    Commandes.Add(cmd);
            }
            catch (Exception ex)
            {
                LogException("Erreur ReloadCommandes", ex);
            }
        }

        private void RefreshClientsList()
        {
            Clients = _context.Clients
                .Include(c => c.IduserNavigation)
                .ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // LOGS (identiques à ton code)
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