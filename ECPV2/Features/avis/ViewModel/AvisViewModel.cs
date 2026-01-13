using ECPV2.Core.ViewModels;
using ECPV2.Domain.Models;
using ECPV2.Services.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ECPV2.Features.avis.ViewModel
{
    internal class AvisViewModel : BaseViewModel, IAvisViewModel
    {
        private string _commentaireSearch = string.Empty;
        private Avi? _avisSelected;
        private bool _editable = false;

        private ObservableCollection<Avi> _avisList = new();

        public string CommentaireSearch
        {
            get => _commentaireSearch;
            set => SetProperty(ref _commentaireSearch, value);
        }

        public ObservableCollection<Avi> AvisList
        {
            get => _avisList;
            set => SetProperty(ref _avisList, value);
        }

        public Avi? AvisSelected
        {
            get => _avisSelected;
            set
            {
                if (SetProperty(ref _avisSelected, value))
                {
                    (CommandAvisEdit as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandAvisDelete as RelayCommand)?.RaiseCanExecuteChanged();
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
                    (CommandAvisNew as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandAvisEdit as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandAvisSave as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandAvisDelete as RelayCommand)?.RaiseCanExecuteChanged();
                    (CommandAvisCancel as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand CommandAvisNew { get; }
        public ICommand CommandAvisEdit { get; }
        public ICommand CommandAvisSave { get; }
        public ICommand CommandAvisDelete { get; }
        public ICommand CommandAvisSearch { get; }
        public ICommand CommandAvisCancel { get; }

        private readonly EcpContext _context;

        public AvisViewModel()
        {
            try
            {
                EnsureLogDirectory();
                _context = new EcpContext();

                CommandAvisNew = new RelayCommand(_ => ActionAvisNew(), _ => CanAvisNew());
                CommandAvisEdit = new RelayCommand(_ => ActionAvisEdit(), _ => CanAvisEdit());
                CommandAvisSave = new RelayCommand(_ => ActionAvisSave(), _ => CanAvisSave());
                CommandAvisDelete = new RelayCommand(_ => ActionAvisDelete(), _ => CanAvisDelete());
                CommandAvisSearch = new RelayCommand(_ => ActionAvisSearch(), _ => CanAvisSearch());
                CommandAvisCancel = new RelayCommand(_ => ActionAvisCancel(), _ => CanAvisCancel());

                var avisList = _context.Avis
                    .Include(a => a.RefpdsNavigation)
                    .Include(a => a.IduserNavigation)
                    .ToList();
                AvisList = new ObservableCollection<Avi>(avisList);
            }
            catch (Exception ex)
            {
                LogException("Erreur initialisation AvisViewModel", ex);
                AvisList = new ObservableCollection<Avi>();
            }
        }

        public void ActionAvisNew()
        {
            try
            {
                AvisSelected = new Avi
                {
                    Refpds = 0,
                    Iduser = 0,
                    Commentaire = string.Empty,
                    RefpdsNavigation = new Produit(),
                    IduserNavigation = new Admin()
                };
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAvisNew", ex);
            }
        }

        public bool CanAvisNew() => !IsEditable;

        public void ActionAvisEdit()
        {
            try
            {
                IsEditable = true;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAvisEdit", ex);
            }
        }

        private bool CanAvisEdit() => AvisSelected != null && !IsEditable;

        private bool CanAvisSave() => IsEditable;

        public void ActionAvisSave()
        {
            try
            {
                if (AvisSelected == null) return;

                if (AvisSelected.Refpds == 0 || AvisSelected.Iduser == 0)
                {
                    // Nouvelle entité
                    _context.Avis.Add(AvisSelected);
                }
                else
                {
                    // Mise à jour
                    _context.Avis.Update(AvisSelected);
                }

                _context.SaveChanges();
                IsEditable = false;
                RefreshAvisList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAvisSave", ex);
            }
        }

        public void ActionAvisDelete()
        {
            try
            {
                if (AvisSelected != null)
                {
                    _context.Avis.Remove(AvisSelected);
                    _context?.SaveChanges();
                }
                RefreshAvisList();
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAvisDelete", ex);
            }
        }

        private bool CanAvisDelete() => AvisSelected != null && !IsEditable;

        private bool CanAvisSearch() => true;

        public void ActionAvisSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CommentaireSearch))
                {
                    RefreshAvisList();
                }
                else
                {
                    var search = CommentaireSearch.ToLower();
                    var results = _context.Avis
                        .Include(a => a.RefpdsNavigation)
                        .Include(a => a.IduserNavigation)
                        .Where(a => a.Commentaire.ToLower().Contains(search))
                        .ToList();
                    AvisList = new ObservableCollection<Avi>(results);
                }
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAvisSearch", ex);
                AvisList = new ObservableCollection<Avi>();
            }
        }

        private bool CanAvisCancel() => IsEditable;

        public void ActionAvisCancel()
        {
            try
            {
                if (AvisSelected != null)
                {
                    _context.Entry(AvisSelected).Reload();
                    if (AvisSelected.RefpdsNavigation != null)
                        _context.Entry(AvisSelected.RefpdsNavigation).Reload();
                    if (AvisSelected.IduserNavigation != null)
                        _context.Entry(AvisSelected.IduserNavigation).Reload();
                }
                IsEditable = false;
            }
            catch (Exception ex)
            {
                LogException("Erreur ActionAvisCancel", ex);
            }
        }

        private void RefreshAvisList()
        {
            var avisList = _context.Avis
                .Include(a => a.RefpdsNavigation)
                .Include(a => a.IduserNavigation)
                .ToList();
            AvisList = new ObservableCollection<Avi>(avisList);
        }

        public override void Dispose()
        {
            _context?.Dispose();
            base.Dispose();
        }
    }
}
