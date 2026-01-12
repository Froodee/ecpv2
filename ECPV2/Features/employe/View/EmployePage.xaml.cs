using Microsoft.UI.Xaml.Controls;
using ECPV2.Features.employe.ViewModel;

namespace ECPV2.Features.employe.View
{
    public sealed partial class EmployePage : Page
    {
        IEmployeViewModel imvm;
        public EmployePage()
        {
            InitializeComponent();
            imvm = new EmployeViewModel();
            DataContext = imvm;
        }
    }
}
