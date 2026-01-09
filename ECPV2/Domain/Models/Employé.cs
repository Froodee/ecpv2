using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Employé
{
    public short Iduser { get; set; }

    public decimal Nosecu { get; set; }

    public DateOnly Dateemp { get; set; }

    public string Nomuser { get; set; } = null!;

    public string Adruser { get; set; } = null!;

    public short Cpuser { get; set; }

    public string Villeuser { get; set; } = null!;

    public int Numuser { get; set; }

    public virtual Utilisateur IduserNavigation { get; set; } = null!;
}
