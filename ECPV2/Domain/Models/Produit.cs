using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Produit
{
    public short Refpds { get; set; }

    public short Idpromo { get; set; }

    public short Codetyp { get; set; }

    public short Qtepds { get; set; }

    public decimal Prixpds { get; set; }

    public string Libpds { get; set; } = null!;

    public string Descpds { get; set; } = null!;

    public string Designpds { get; set; } = null!;

    public virtual ICollection<Avi> Avis { get; set; } = new List<Avi>();

    public virtual Typeproduit CodetypNavigation { get; set; } = null!;

    public virtual Promo IdpromoNavigation { get; set; } = null!;

    public virtual ICollection<Commande> Numcdes { get; set; } = new List<Commande>();

    public virtual ICollection<Produit> Refpds1s { get; set; } = new List<Produit>();

    public virtual ICollection<Produit> RefpdsNavigation { get; set; } = new List<Produit>();
}
