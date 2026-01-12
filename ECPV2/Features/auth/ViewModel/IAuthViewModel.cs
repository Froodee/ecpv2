using System.Windows.Input;

namespace ECPV2.Features.auth.ViewModel
{
    internal interface IAuthViewModel
    {
        string Login { get; set; }
        string Password { get; set; }
        string Pin { get; set; }
        string Message { get; set; }
        bool IsPinSent { get; set; }

        ICommand SendPinCommand { get; }
        ICommand ConnectCommand { get; }
    }
}
