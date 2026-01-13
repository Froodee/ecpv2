using ECPV2.Core.ViewModels;
using ECPV2.Domain.Models;
using ECPV2.Services.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ECPV2.Features.promo.ViewModel
{
    internal class PromoViewModel : BaseViewModel, IPromoViewModel
    {
        private string _promoSearch = string.Empty;
        private Promo? _promoSelected;
        private bool _editable = false;

        private ObservableCollection<Promo> _promos = new();

        public string PromoSearch
        {
            get => _promoSearch;
            set => SetProperty(ref _promoSearch, value);
        }

        public ObservableCollection<Promo> Promos
        {
            get => _promos;
            set => SetProperty(ref _promos, value);
        }

        public Promo? PromoSelected
        {
            get => _promoSelected;
            set
            {
                if (SetProperty(ref _promoSelected, value))
                {
                    (CommandPromoEdit as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandPromoDelete as RelayCommand)?.RaiseCanExecuteChanged();
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
                    (CommandPromoNew as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandPromoEdit as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandPromoSave as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandPromoDelete as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandPromoCancel as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand CommandPromoNew { get; }
        public ICommand CommandPromoEdit { get; }
        public ICommand CommandPromoSave { get; }
        public ICommand CommandPromoDelete { get; }
        public ICommand CommandPromoSearch { get; }
        public ICommand CommandPromoCancel { get; }

        private readonly EcpContext _context;

        public PromoViewModel()
        {
            try
            {
                EnsureLogDirectory();
                _context = new EcpContext();

                CommandPromoNew = new RelayCommand(_ => ActionPromoNew(), _ => CanPromoNew());
                CommandPromoEdit = new RelayCommand(_ => ActionPromoEdit(), _ => CanPromoEdit());
                CommandPromoSave = new RelayCommand(_ => ActionPromoSave(), _ => CanPromoSave());
                CommandPromoDelete = new RelayCommand(_ => ActionPromoDelete(), _ => CanPromoDelete());
                CommandPromoSearch = new RelayCommand(_ => ActionPromoSearch(), _ => CanPromoSearch());
                CommandPromoCancel = new RelayCommand(_ => ActionPromoCancel(), _ => CanPromoCancel());

                var promosList = _context.Promos
                    .Include(p => p.Produits)
                    .ToList();
                Promos = new ObservableCollection<Promo>(promosList);
            }
            catch (Exception ex)
            {
                LogException("Erreur initialisation PromoViewModel", ex);
                Promos = new ObservableCollection<Promo>();
            }
        }

        public void ActionPromoNew()
        {
            try
            {
                PromoSelected = new Promo
                {
                    Idpromo = 0,
                    Datedeb = DateOnly.FromDateTime(DateTime.Now),
                    Datefin = DateOnly.FromDateTime(DateTime.Now),
                    Reduc = 0m
                };
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionPromoNew", ex);
            }
        }

        public bool CanPromoNew() => !IsEditable;

        public void ActionPromoEdit()
        {
            try
            {
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionPromoEdit", ex);
            }
        }

        private bool CanPromoEdit() => PromoSelected != null && !IsEditable;

        private bool CanPromoSave() => IsEditable;

        public void ActionPromoSave()
        {
            try
            {
                if (PromoSelected == null) return;

                if (PromoSelected.Idpromo == 0)
                {
                    // Création
                    _context.Promos.Add(PromoSelected);
                }
                else
                {
                    // Mise à jour
                    _context.Promos.Update(PromoSelected);
                }

                _context.SaveChanges();
                IsEditable = false;
                RefreshPromosList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionPromoSave", ex);
            }
        }

        public void ActionPromoDelete()
        {
            try
            {
                if (PromoSelected != null)
                {
                    _context.Promos.Remove(PromoSelected);
                    _context?.SaveChanges();
                }
                RefreshPromosList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionPromoDelete", ex);
            }
        }

        private bool CanPromoDelete() => PromoSelected != null && !IsEditable;

        private bool CanPromoSearch() => true;

        public void ActionPromoSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PromoSearch))
                {
                    RefreshPromosList();
                }
                else
                {
                    var search = PromoSearch.ToLower();
                    var results = _context.Promos
                        .Include(p => p.Produits)
                        .Where(p => p.Reduc.ToString().Contains(search))
                        .ToList();
                    Promos = new ObservableCollection<Promo>(results);
                }
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionPromoSearch", ex);
                Promos = new ObservableCollection<Promo>();
            }
        }

        private bool CanPromoCancel() => IsEditable;

        public void ActionPromoCancel()
        {
            try
            {
                if (PromoSelected != null)
                {
                    _context.Entry(PromoSelected).Reload();
                }
                IsEditable = false;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionPromoCancel", ex);
            }
        }

        private void RefreshPromosList()
        {
            var promosList = _context.Promos
                .Include(p => p.Produits)
                .ToList();
            Promos = new ObservableCollection<Promo>(promosList);
        }

        public override void Dispose()
        {
            _context?.Dispose();
            base.Dispose();
        }
    }
}
