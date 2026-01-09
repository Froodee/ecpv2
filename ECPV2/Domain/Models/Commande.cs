using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Commande
{
    public short Numcde { get; set; }

    public short Iduser { get; set; }

    public DateOnly Datecde { get; set; }

    public virtual ICollection<Facture> Factures { get; set; } = new List<Facture>();

    public virtual Utilisateur IduserNavigation { get; set; } = null!;

    public virtual ICollection<Produit> Refpds { get; set; } = new List<Produit>();
}
