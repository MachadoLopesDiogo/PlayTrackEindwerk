namespace PlayTrackEindwerk.Models
{
    public class WedstrijdRequest
    {
        public string Datum { get; set; } = string.Empty;

        public int ThuisScore { get; set; }
        public int UitScore { get; set; }

        public string? ThuisNaam { get; set; }
        public string? UitNaam { get; set; }

        // Speler statistieken
        public int Goals { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int Geel { get; set; } = 0;
        public int Rood { get; set; } = 0;
        public int Fouten { get; set; } = 0;
        public int Aanvallen { get; set; } = 0;
        public int Tackles { get; set; } = 0;
        public int? Rating { get; set; }
    }
}