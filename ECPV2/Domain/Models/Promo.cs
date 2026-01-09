using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Promo
{
    public DateOnly Datedeb { get; set; }

    public DateOnly Datefin { get; set; }

    public decimal Reduc { get; set; }

    public short Idpromo { get; set; }

    public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();
}
