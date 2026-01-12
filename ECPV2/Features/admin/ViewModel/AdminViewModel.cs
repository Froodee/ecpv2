using ECPV2.Core.ViewModels;
using ECPV2.Domain.Models;
using ECPV2.Services.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ECPV2.Features.admin.ViewModel
{
    internal class AdminViewModel : BaseViewModel, IAdminViewModel
    {
        private string _nomSearch = string.Empty;
        private Admin? _adminSelected;
        private bool _editable = false;

        private ObservableCollection<Admin> _admins = new();
        private ObservableCollection<Avi> _avis = new();

        public string NomSearch
        {
            get => _nomSearch;
            set => SetProperty(ref _nomSearch, value);
        }

        public ObservableCollection<Admin> Admins
        {
            get => _admins;
            set => SetProperty(ref _admins, value);
        }

        public Admin? AdminSelected
        {
            get => _adminSelected;
            set
            {
                if (SetProperty(ref _adminSelected, value))
                    ReloadAvis();
            }
        }

        public ObservableCollection<Avi> Avis
        {
            get => _avis;
        }

        public bool IsEditable
        {
            get => _editable;
            set => SetProperty(ref _editable, value);
        }

        public ICommand CommandAdminNew { get; set; }
        public ICommand CommandAdminEdit { get; set; }
        public ICommand CommandAdminSave { get; set; }
        public ICommand CommandAdminDelete { get; set; }
        public ICommand CommandAdminSearch { get; set; }
        public ICommand CommandAdminCancel { get; set; }

        private readonly EcpContext _context;

        public AdminViewModel()
        {
            try
            {
                EnsureLogDirectory();
                _context = new EcpContext();

                CommandAdminNew = new RelayCommand(_ => ActionAdminNew(), _ => CanAdminNew());
                CommandAdminEdit = new RelayCommand(_ => ActionAdminEdit(), _ => CanAdminEdit());
                CommandAdminSave = new RelayCommand(_ => ActionAdminSave(), _ => CanAdminSave());
                CommandAdminDelete = new RelayCommand(_ => ActionAdminDelete(), _ => CanAdminDelete());
                CommandAdminSearch = new RelayCommand(_ => ActionAdminSearch(), _ => CanAdminSearch());
                CommandAdminCancel = new RelayCommand(_ => ActionAdminCancel(), _ => CanAdminCancel());

                var adminsList = _context.Admins
                    .Include(a => a.IduserNavigation)
                    .ToList();
                Admins = new ObservableCollection<Admin>(adminsList);
            }
            catch (Exception ex)
            {
                LogException("Erreur initialisation AdminViewModel", ex);
                Admins = new ObservableCollection<Admin>();
            }
        }

        public void ActionAdminNew()
        {
            try
            {
                var utilisateur = new Utilisateur
                {
                    Nomuser = string.Empty,
                    Adruser = string.Empty,
                    Cpuser = 0,
                    Villeuser = string.Empty,
                    Numuser = 0
                };
                AdminSelected = new Admin
                {
                    IduserNavigation = utilisateur
                };
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAdminNew", ex);
            }
        }

        public bool CanAdminNew() => true;

        public void ActionAdminEdit()
        {
            try
            {
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAdminEdit", ex);
            }
        }

        private bool CanAdminEdit() => true;

        private bool CanAdminSave() => true;

        public void ActionAdminSave()
        {
            try
            {
                if (AdminSelected == null) return;

                if (AdminSelected.Iduser == 0)
                {
                    var utilisateur = new Utilisateur
                    {
                        Nomuser = AdminSelected.Nomuser,
                        Adruser = AdminSelected.Adruser,
                        Cpuser = (short)AdminSelected.Cpuser,
                        Villeuser = AdminSelected.Villeuser,
                        Numuser = AdminSelected.Numuser
                    };

                    var admin = new Admin
                    {
                        IduserNavigation = utilisateur
                    };

                    _context.Utilisateurs.Add(utilisateur);
                    _context.SaveChanges();

                    admin.Iduser = utilisateur.Iduser;
                    _context.Admins.Add(admin);
                }
                else
                {
                    _context.Utilisateurs.Update(AdminSelected.IduserNavigation);
                    _context.Admins.Update(AdminSelected);
                }

                _context.SaveChanges();
                IsEditable = false;
                RefreshAdminsList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAdminSave", ex);
            }
        }

        public void ActionAdminDelete()
        {
            try
            {
                if (AdminSelected != null)
                {
                    var utilisateur = AdminSelected.IduserNavigation;
                    _context.Admins.Remove(AdminSelected);
                    if (utilisateur != null)
                        _context.Utilisateurs.Remove(utilisateur);
                    _context?.SaveChanges();
                }
                RefreshAdminsList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAdminDelete", ex);
            }
        }

        private bool CanAdminDelete() => AdminSelected != null && !IsEditable;

        private bool CanAdminSearch() => true;

        public void ActionAdminSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NomSearch))
                {
                    RefreshAdminsList();
                }
                else
                {
                    var search = NomSearch.ToLower();
                    var results = _context.Admins
                        .Include(a => a.IduserNavigation)
                        .Where(a => a.Nomuser.ToLower().Contains(search) ||
                                   a.IduserNavigation.Villeuser.ToLower().Contains(search))
                        .ToList();
                    Admins = new ObservableCollection<Admin>(results);
                }
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAdminSearch", ex);
                Admins = new ObservableCollection<Admin>();
            }
        }

        private bool CanAdminCancel() => IsEditable;

        public void ActionAdminCancel()
        {
            try
            {
                if (AdminSelected != null)
                {
                    _context.Entry(AdminSelected).Reload();
                    if (AdminSelected.IduserNavigation != null)
                        _context.Entry(AdminSelected.IduserNavigation).Reload();
                }
                IsEditable = false;
                ReloadAvis();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAdminCancel", ex);
            }
        }

        private void ReloadAvis()
        {
            try
            {
                Avis.Clear();
                if (AdminSelected == null || AdminSelected.Iduser == 0) return;

                var avis = _context.Avis
                    .Where(a => a.Iduser == AdminSelected.Iduser)
                    .ToList();

                foreach (var avi in avis)
                    Avis.Add(avi);
            }
            catch (Exception ex)
            {
                LogException("Erreur ReloadAvis", ex);
            }
        }

        private void RefreshAdminsList()
        {
            var adminsList = _context.Admins
                .Include(a => a.IduserNavigation)
                .ToList();
            Admins = new ObservableCollection<Admin>(adminsList);
        }

        public override void Dispose()
        {
            _context?.Dispose();
            base.Dispose();
        }
    }
}
