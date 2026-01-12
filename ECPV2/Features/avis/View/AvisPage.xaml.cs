using Microsoft.UI.Xaml.Controls;
using ECPV2.Features.avis.ViewModel;

namespace ECPV2.Features.avis.View
{
    public sealed partial class AvisPage : Page
    {
        IAvisViewModel imvm;
        public AvisPage()
        {
            InitializeComponent();
            imvm = new AvisViewModel();
            DataContext = imvm;
        }
    }
}
