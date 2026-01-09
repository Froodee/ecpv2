using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Avi
{
    public short Refpds { get; set; }

    public short Iduser { get; set; }

    public string Commentaire { get; set; } = null!;

    public virtual Admin IduserNavigation { get; set; } = null!;

    public virtual Produit RefpdsNavigation { get; set; } = null!;
}
