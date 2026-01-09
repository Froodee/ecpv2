using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Typeproduit
{
    public short Codetyp { get; set; }

    public string Libtyp { get; set; } = null!;

    public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();
}
