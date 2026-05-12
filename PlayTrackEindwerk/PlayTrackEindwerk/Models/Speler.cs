using System;
using System.Collections.Generic;

namespace PlayTrackEindwerk.Models;

public partial class Speler
{
    public int Idspeler { get; set; }

    public string SpelerVoornaam { get; set; } = null!;

    public string SpelerAchternaam { get; set; } = null!;

    public string Positie { get; set; } = null!;

    public virtual ICollection<Wedstrijdheeftspeler> Wedstrijdheeftspelers { get; set; } = new List<Wedstrijdheeftspeler>();
}
