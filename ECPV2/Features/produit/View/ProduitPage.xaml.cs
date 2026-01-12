using Microsoft.UI.Xaml.Controls;
using ECPV2.Features.produit.ViewModel;

namespace ECPV2.Features.produit.View
{
    public sealed partial class ProduitPage : Page
    {
        IProduitViewModel imvm;
        public ProduitPage()
        {
            InitializeComponent();
            imvm = new ProduitViewModel();
            DataContext = imvm;
        }
    }
}
