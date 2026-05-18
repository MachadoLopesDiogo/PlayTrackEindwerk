using System.ComponentModel.DataAnnotations;

namespace PlayTrackEindwerk.Models
{
    public class Speler
    {
        [Key]
        public int Idspeler { get; set; }

        public string SpelerVoornaam { get; set; } = string.Empty;
        public string SpelerAchternaam { get; set; } = string.Empty;
        public string Positie { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;

        public virtual ICollection<Wedstrijdheeftspeler> Wedstrijdheeftspelers { get; set; } = new List<Wedstrijdheeftspeler>();
    }
}