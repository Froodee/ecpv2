using Microsoft.UI.Xaml.Controls;
using ECPV2.Features.admin.ViewModel;

namespace ECPV2.Features.admin.View
{
    public sealed partial class AdminPage : Page
    {
        IAdminViewModel imvm;
        public AdminPage()
        {
            InitializeComponent();
            imvm = new AdminViewModel();
            DataContext = imvm;
        }
    }
}
