using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Utilisateur
{
    public short Iduser { get; set; }

    public string Nomuser { get; set; } = null!;

    public string Adruser { get; set; } = null!;

    public int Cpuser { get; set; }

    public string Villeuser { get; set; } = null!;

    public int Numuser { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();

    public virtual Employé? Employé { get; set; }

    public virtual ICollection<Connexion> Idus { get; set; } = new List<Connexion>();
}
