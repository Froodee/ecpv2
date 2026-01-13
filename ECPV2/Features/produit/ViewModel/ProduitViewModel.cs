using ECPV2.Core.ViewModels;
using ECPV2.Domain.Models;
using ECPV2.Services.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ECPV2.Features.produit.ViewModel
{
    internal class ProduitViewModel : BaseViewModel, IProduitViewModel
    {
        private string _libelleSearch = string.Empty;
        private Produit? _produitSelected;
        private bool _editable = false;

        private ObservableCollection<Produit> _produits = new();

        public string LibelleSearch
        {
            get => _libelleSearch;
            set => SetProperty(ref _libelleSearch, value);
        }

        public ObservableCollection<Produit> Produits
        {
            get => _produits;
            set => SetProperty(ref _produits, value);
        }

        public Produit? ProduitSelected
        {
            get => _produitSelected;
            set
            {
                if (SetProperty(ref _produitSelected, value))
                {
                    (CommandProduitEdit as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandProduitDelete as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsEditable
        {
            get => _editable;
            set
            {
                if (SetProperty(ref _editable, value))
                {
                    (CommandProduitNew as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandProduitEdit as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandProduitSave as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandProduitDelete as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandProduitCancel as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand CommandProduitNew { get; }
        public ICommand CommandProduitEdit { get; }
        public ICommand CommandProduitSave { get; }
        public ICommand CommandProduitDelete { get; }
        public ICommand CommandProduitSearch { get; }
        public ICommand CommandProduitCancel { get; }

        private readonly EcpContext _context;

        public ProduitViewModel()
        {
            try
            {
                EnsureLogDirectory();
                _context = new EcpContext();

                CommandProduitNew = new RelayCommand(_ => ActionProduitNew(), _ => CanProduitNew());
                CommandProduitEdit = new RelayCommand(_ => ActionProduitEdit(), _ => CanProduitEdit());
                CommandProduitSave = new RelayCommand(_ => ActionProduitSave(), _ => CanProduitSave());
                CommandProduitDelete = new RelayCommand(_ => ActionProduitDelete(), _ => CanProduitDelete());
                CommandProduitSearch = new RelayCommand(_ => ActionProduitSearch(), _ => CanProduitSearch());
                CommandProduitCancel = new RelayCommand(_ => ActionProduitCancel(), _ => CanProduitCancel());

                var produitsList = _context.Produits
                    .Include(p => p.CodetypNavigation)
                    .Include(p => p.IdpromoNavigation)
                    .ToList();
                Produits = new ObservableCollection<Produit>(produitsList);
            }
            catch (Exception ex)
            {
                LogException("Erreur initialisation ProduitViewModel", ex);
                Produits = new ObservableCollection<Produit>();
            }
        }

        public void ActionProduitNew()
        {
            try
            {
                ProduitSelected = new Produit
                {
                    Refpds = 0,
                    Idpromo = 0,
                    Codetyp = 0,
                    Qtepds = 0,
                    Prixpds = 0,
                    Libpds = string.Empty,
                    Descpds = string.Empty,
                    Designpds = string.Empty
                };
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionProduitNew", ex);
            }
        }

        public bool CanProduitNew() => !IsEditable;

        public void ActionProduitEdit()
        {
            try
            {
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionProduitEdit", ex);
            }
        }

        private bool CanProduitEdit() => ProduitSelected != null && !IsEditable;

        private bool CanProduitSave() => IsEditable;

        public void ActionProduitSave()
        {
            try
            {
                if (ProduitSelected == null) return;

                if (ProduitSelected.Refpds == 0)
                {
                    _context.Produits.Add(ProduitSelected);
                }
                else
                {
                    _context.Produits.Update(ProduitSelected);
                }

                _context.SaveChanges();
                IsEditable = false;
                RefreshProduitsList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionProduitSave", ex);
            }
        }

        public void ActionProduitDelete()
        {
            try
            {
                if (ProduitSelected != null)
                {
                    _context.Produits.Remove(ProduitSelected);
                    _context?.SaveChanges();
                }
                RefreshProduitsList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionProduitDelete", ex);
            }
        }

        private bool CanProduitDelete() => ProduitSelected != null && !IsEditable;

        private bool CanProduitSearch() => true;

        public void ActionProduitSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LibelleSearch))
                {
                    RefreshProduitsList();
                }
                else
                {
                    var search = LibelleSearch.ToLower();
                    var results = _context.Produits
                        .Include(p => p.CodetypNavigation)
                        .Include(p => p.IdpromoNavigation)
                        .Where(p => p.Libpds.ToLower().Contains(search) ||
                                   p.Designpds.ToLower().Contains(search) ||
                                   p.Descpds.ToLower().Contains(search))
                        .ToList();
                    Produits = new ObservableCollection<Produit>(results);
                }
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionProduitSearch", ex);
                Produits = new ObservableCollection<Produit>();
            }
        }

        private bool CanProduitCancel() => IsEditable;

        public void ActionProduitCancel()
        {
            try
            {
                if (ProduitSelected != null)
                {
                    _context.Entry(ProduitSelected).Reload();
                    if (ProduitSelected.CodetypNavigation != null)
                        _context.Entry(ProduitSelected.CodetypNavigation).Reload();
                    if (ProduitSelected.IdpromoNavigation != null)
                        _context.Entry(ProduitSelected.IdpromoNavigation).Reload();
                }
                IsEditable = false;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionProduitCancel", ex);
            }
        }

        private void RefreshProduitsList()
        {
            var produitsList = _context.Produits
                .Include(p => p.CodetypNavigation)
                .Include(p => p.IdpromoNavigation)
                .ToList();
            Produits = new ObservableCollection<Produit>(produitsList);
        }

        public override void Dispose()
        {
            _context?.Dispose();
            base.Dispose();
        }
    }
}
