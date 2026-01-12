using ECPV2.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ECPV2.Features.produit.ViewModel
{
    internal interface IProduitViewModel
    {
        string LibelleSearch { get; set; }

        ObservableCollection<Produit> Produits { get; set; }
        Produit? ProduitSelected { get; set; }

        bool IsEditable { get; set; }

        // Commandes
        ICommand CommandProduitNew { get; }
        ICommand CommandProduitEdit { get; }
        ICommand CommandProduitSave { get; }
        ICommand CommandProduitDelete { get; }
        ICommand CommandProduitSearch { get; }
        ICommand CommandProduitCancel { get; }
    }
}
