using ECPV2.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ECPV2.Features.avis.ViewModel
{
    internal interface IAvisViewModel
    {
        string CommentaireSearch { get; set; }

        ObservableCollection<Avi> AvisList { get; set; }
        Avi? AvisSelected { get; set; }

        bool IsEditable { get; set; }

        // Commandes
        ICommand CommandAvisNew { get; }
        ICommand CommandAvisEdit { get; }
        ICommand CommandAvisSave { get; }
        ICommand CommandAvisDelete { get; }
        ICommand CommandAvisSearch { get; }
        ICommand CommandAvisCancel { get; }
    }
}
