using ECPV2.Domain.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ECPV2.Features.employe.ViewModel
{
    internal interface IEmployeViewModel
    {
        string NomSearch { get; set; }

        ObservableCollection<Employé> Employes { get; set; }
        Employé? EmployeSelected { get; set; }

        bool IsEditable { get; set; }

        // Commandes
        ICommand CommandEmployeNew { get; }
        ICommand CommandEmployeEdit { get; }
        ICommand CommandEmployeSave { get; }
        ICommand CommandEmployeDelete { get; }
        ICommand CommandEmployeSearch { get; }
        ICommand CommandEmployeCancel { get; }
    }
}
