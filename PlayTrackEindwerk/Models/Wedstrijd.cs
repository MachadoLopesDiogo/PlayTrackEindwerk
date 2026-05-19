using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;  // ← NIEUW

namespace PlayTrackEindwerk.Models
{
    public class Wedstrijd
    {
        [Key]
        public int Idwedstrijd { get; set; }

        public DateOnly Datum { get; set; }

        // Team namen
        public string ThuisTeam { get; set; } = string.Empty;
        public string UitTeam { get; set; } = string.Empty;
        public string ThuisNaam { get; set; } = string.Empty;
        public string UitNaam { get; set; } = string.Empty;

        // Score (bijv. "3-1")
        public string Score { get; set; } = string.Empty;

        [NotMapped]  
        public string Seizoen { get; set; } = string.Empty;
       

        // Navigatie property
        public virtual ICollection<Wedstrijdheeftspeler> Wedstrijdheeftspelers { get; set; }
            = new List<Wedstrijdheeftspeler>();
    }
}