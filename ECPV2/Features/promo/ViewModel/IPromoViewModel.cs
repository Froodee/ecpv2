using ECPV2.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ECPV2.Features.promo.ViewModel
{
    internal interface IPromoViewModel
    {
        string PromoSearch { get; set; }

        ObservableCollection<Promo> Promos { get; set; }
        Promo? PromoSelected { get; set; }

        bool IsEditable { get; set; }

        // Commandes
        ICommand CommandPromoNew { get; }
        ICommand CommandPromoEdit { get; }
        ICommand CommandPromoSave { get; }
        ICommand CommandPromoDelete { get; }
        ICommand CommandPromoSearch { get; }
        ICommand CommandPromoCancel { get; }
    }
}
