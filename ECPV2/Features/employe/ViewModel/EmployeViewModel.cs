using ECPV2.Core.ViewModels;
using ECPV2.Domain.Models;
using ECPV2.Services.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ECPV2.Features.employe.ViewModel
{
    internal class EmployeViewModel : BaseViewModel, IEmployeViewModel
    {
        private string _nomSearch = string.Empty;
        private Employé? _employeSelected;
        private bool _editable = false;

        private ObservableCollection<Employé> _employes = new();

        public string NomSearch
        {
            get => _nomSearch;
            set => SetProperty(ref _nomSearch, value);
        }

        public ObservableCollection<Employé> Employes
        {
            get => _employes;
            set => SetProperty(ref _employes, value);
        }

        public Employé? EmployeSelected
        {
            get => _employeSelected;
            set => SetProperty(ref _employeSelected, value);
        }

        public bool IsEditable
        {
            get => _editable;
            set => SetProperty(ref _editable, value);
        }

        public ICommand CommandEmployeNew { get; set; }
        public ICommand CommandEmployeEdit { get; set; }
        public ICommand CommandEmployeSave { get; set; }
        public ICommand CommandEmployeDelete { get; set; }
        public ICommand CommandEmployeSearch { get; set; }
        public ICommand CommandEmployeCancel { get; set; }

        private readonly EcpContext _context;

        public EmployeViewModel()
        {
            try
            {
                EnsureLogDirectory();
                _context = new EcpContext();

                CommandEmployeNew = new RelayCommand(_ => ActionEmployeNew(), _ => CanEmployeNew());
                CommandEmployeEdit = new RelayCommand(_ => ActionEmployeEdit(), _ => CanEmployeEdit());
                CommandEmployeSave = new RelayCommand(_ => ActionEmployeSave(), _ => CanEmployeSave());
                CommandEmployeDelete = new RelayCommand(_ => ActionEmployeDelete(), _ => CanEmployeDelete());
                CommandEmployeSearch = new RelayCommand(_ => ActionEmployeSearch(), _ => CanEmployeSearch());
                CommandEmployeCancel = new RelayCommand(_ => ActionEmployeCancel(), _ => CanEmployeCancel());

                var employesList = _context.Employés
                    .Include(e => e.IduserNavigation)
                    .ToList();
                Employes = new ObservableCollection<Employé>(employesList);
            }
            catch (Exception ex)
            {
                LogException("Erreur initialisation EmployeViewModel", ex);
                Employes = new ObservableCollection<Employé>();
            }
        }

        public void ActionEmployeNew()
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
                EmployeSelected = new Employé
                {
                    IduserNavigation = utilisateur,
                    Nosecu = 0,
                    Dateemp = DateOnly.FromDateTime(DateTime.Now)
                };
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionEmployeNew", ex);
            }
        }

        public bool CanEmployeNew() => true;

        public void ActionEmployeEdit()
        {
            try
            {
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionEmployeEdit", ex);
            }
        }

        private bool CanEmployeEdit() => true;

        private bool CanEmployeSave() => true;

        public void ActionEmployeSave()
        {
            try
            {
                if (EmployeSelected == null) return;

                if (EmployeSelected.Iduser == 0)
                {
                    var utilisateur = new Utilisateur
                    {
                        Nomuser = EmployeSelected.Nomuser,
                        Adruser = EmployeSelected.Adruser,
                        Cpuser = (short)EmployeSelected.Cpuser,
                        Villeuser = EmployeSelected.Villeuser,
                        Numuser = EmployeSelected.Numuser
                    };

                    var employe = new Employé
                    {
                        Nosecu = EmployeSelected.Nosecu,
                        Dateemp = EmployeSelected.Dateemp,
                        IduserNavigation = utilisateur
                    };

                    _context.Utilisateurs.Add(utilisateur);
                    _context.SaveChanges();

                    employe.Iduser = utilisateur.Iduser;
                    _context.Employés.Add(employe);
                }
                else
                {
                    _context.Utilisateurs.Update(EmployeSelected.IduserNavigation);
                    _context.Employés.Update(EmployeSelected);
                }

                _context.SaveChanges();
                IsEditable = false;
                RefreshEmployesList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionEmployeSave", ex);
            }
        }

        public void ActionEmployeDelete()
        {
            try
            {
                if (EmployeSelected != null)
                {
                    var utilisateur = EmployeSelected.IduserNavigation;
                    _context.Employés.Remove(EmployeSelected);
                    if (utilisateur != null)
                        _context.Utilisateurs.Remove(utilisateur);
                    _context?.SaveChanges();
                }
                RefreshEmployesList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionEmployeDelete", ex);
            }
        }

        private bool CanEmployeDelete() => EmployeSelected != null && !IsEditable;

        private bool CanEmployeSearch() => true;

        public void ActionEmployeSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NomSearch))
                {
                    RefreshEmployesList();
                }
                else
                {
                    var search = NomSearch.ToLower();
                    var results = _context.Employés
                        .Include(e => e.IduserNavigation)
                        .Where(e => e.Nomuser.ToLower().Contains(search) ||
                                   e.Nosecu.ToString().Contains(search) ||
                                   e.IduserNavigation.Villeuser.ToLower().Contains(search))
                        .ToList();
                    Employes = new ObservableCollection<Employé>(results);
                }
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionEmployeSearch", ex);
                Employes = new ObservableCollection<Employé>();
            }
        }

        private bool CanEmployeCancel() => IsEditable;

        public void ActionEmployeCancel()
        {
            try
            {
                if (EmployeSelected != null)
                {
                    _context.Entry(EmployeSelected).Reload();
                    if (EmployeSelected.IduserNavigation != null)
                        _context.Entry(EmployeSelected.IduserNavigation).Reload();
                }
                IsEditable = false;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionEmployeCancel", ex);
            }
        }

        private void RefreshEmployesList()
        {
            var employesList = _context.Employés
                .Include(e => e.IduserNavigation)
                .ToList();
            Employes = new ObservableCollection<Employé>(employesList);
        }

        public override void Dispose()
        {
            _context?.Dispose();
            base.Dispose();
        }
    }
}
