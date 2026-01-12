using Microsoft.UI.Xaml.Controls;
using ECPV2.Features.auth.ViewModel;

namespace ECPV2.Features.auth.View
{
    public sealed partial class AuthPage : Page
    {
        IAuthViewModel imvm;

        public AuthViewModel ViewModel { get; }

        public AuthPage()
        {
            InitializeComponent();
            ViewModel = new AuthViewModel();
            imvm = ViewModel;
            DataContext = imvm;
        }
    }
}
