using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Connexion
{
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public short Idu { get; set; }

    public virtual ICollection<Utilisateur> Idusers { get; set; } = new List<Utilisateur>();
}
