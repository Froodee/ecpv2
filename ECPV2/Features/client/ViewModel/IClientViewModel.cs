using ECPV2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ECPV2.Features.client.ViewModel
{
    internal interface IClientViewModel
    {
        string NomSearch { get; set; }

        List<Client> Clients { get; set; }
        Client ClientSelected { get; set; }

        ObservableCollection<Commande> Commandes { get; }   
        Commande CommandeSelected { get; set; }

        bool IsEditable { get; set; }

        // Commandes
        ICommand CommandClientNew { get; }
        ICommand CommandClientEdit { get; }
        ICommand CommandClientSave { get; }
        ICommand CommandClientDelete { get; }
        ICommand CommandClientSearch { get; }
        ICommand CommandClientCancel { get; }
    }
}
