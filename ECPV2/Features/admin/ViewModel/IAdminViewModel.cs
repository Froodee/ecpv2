using ECPV2.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ECPV2.Features.admin.ViewModel
{
    internal interface IAdminViewModel
    {
        string NomSearch { get; set; }

        ObservableCollection<Admin> Admins { get; set; }
        Admin? AdminSelected { get; set; }

        ObservableCollection<Avi> Avis { get; }

        bool IsEditable { get; set; }

        // Commandes
        ICommand CommandAdminNew { get; }
        ICommand CommandAdminEdit { get; }
        ICommand CommandAdminSave { get; }
        ICommand CommandAdminDelete { get; }
        ICommand CommandAdminSearch { get; }
        ICommand CommandAdminCancel { get; }
    }
}
