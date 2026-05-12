using PlayTrackEindwerk.Models;
using System;
using System.Collections.Generic;

namespace BucketListAPI.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string NameUser { get; set; } = null!;

    public string PassWordUser { get; set; } = null!;

    public virtual ICollection<Voetbaldb> Voetbaldb { get; set; } = new List<Voetbaldb>();
}