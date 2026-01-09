using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Client
{
    public short Iduser { get; set; }

    public int? Siret { get; set; }

    public string Nomuser { get; set; } = null!;

    public string Adruser { get; set; } = null!;

    public int Cpuser { get; set; }

    public string Villeuser { get; set; } = null!;

    public int Numuser { get; set; }

    public virtual Utilisateur IduserNavigation { get; set; } = null!;
}
