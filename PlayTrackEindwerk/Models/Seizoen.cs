using System;
using System.Collections.Generic;

namespace PlayTrackEindwerk.Models;

public partial class Seizoen
{
    public int Idseizoen { get; set; }
    public int NiveauSeizoen { get; set; }
    public int Fkwedstrijd { get; set; }
    
    public virtual Wedstrijd FkwedstrijdNavigation { get; set; } = null!;
}