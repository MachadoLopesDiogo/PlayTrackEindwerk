using System;
using System.Collections.Generic;

namespace PlayTrackEindwerk.Models;

public partial class Wedstrijd
{
    public int Idwedstrijd { get; set; }

    public DateOnly Datum { get; set; }

    public string Score { get; set; } = null!;

    public virtual ICollection<Seizoen> Seizoens { get; set; } = new List<Seizoen>();

    public virtual ICollection<Wedstrijdheeftspeler> Wedstrijdheeftspelers { get; set; } = new List<Wedstrijdheeftspeler>();
}
