using System;
using System.Collections.Generic;

namespace ECPV2.Domain.Models;

public partial class Facture
{
    public short Numfact { get; set; }

    public short Numcde { get; set; }

    public DateOnly Datefact { get; set; }

    public virtual Commande NumcdeNavigation { get; set; } = null!;
}
