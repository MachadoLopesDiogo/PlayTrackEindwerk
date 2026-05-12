using System;
using System.Collections.Generic;

namespace PlayTrackEindwerk.Models;

public partial class Wedstrijdheeftspeler
{
    public int Fkspeler { get; set; }

    public int Fkwedstrijd { get; set; }

    public int AantalGoals { get; set; }

    public int AantalAssists { get; set; }

    public int AantalGeleRodeKaarten { get; set; }

    public int AantalGrofFouten { get; set; }

    public int AantalBelangrijkeActies { get; set; }

    public int AantalBelangrijkeTackles { get; set; }

    public int RatingOp10 { get; set; }

    public virtual Speler FkspelerNavigation { get; set; } = null!;

    public virtual Wedstrijd FkwedstrijdNavigation { get; set; } = null!;
}
