using Microsoft.EntityFrameworkCore;
using PlayTrackEindwerk.Models;

namespace PlayTrackEindwerk.Models
{
    public partial class Voetbaldb : DbContext
    {
        public Voetbaldb()
        {
        }

        public Voetbaldb(DbContextOptions<Voetbaldb> options)
            : base(options)
        {
        }

        public virtual DbSet<Seizoen> Seizoens { get; set; } = null!;
        public virtual DbSet<Speler> Spelers { get; set; } = null!;
        public virtual DbSet<Wedstrijd> Wedstrijds { get; set; } = null!;
        public virtual DbSet<Wedstrijdheeftspeler> Wedstrijdheeftspelers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;database=voetbaldb;User=root;password=1234",
                    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Seizoen>(entity =>
            {
                entity.HasKey(e => e.Idseizoen).HasName("PRIMARY");
                entity.ToTable("seizoen");
                entity.HasIndex(e => e.Fkwedstrijd, "FKWedstrijd");
                entity.Property(e => e.Idseizoen).HasColumnName("IDSeizoen");
                entity.Property(e => e.Fkwedstrijd).HasColumnName("FKWedstrijd");

                // Tijdelijk uitgeschakeld om error te vermijden
                // entity.HasOne(d => d.FkwedstrijdNavigation)
                //       .WithMany(p => p.Seizoens)
                //       .HasForeignKey(d => d.Fkwedstrijd)
                //       .OnDelete(DeleteBehavior.ClientSetNull)
                //       .HasConstraintName("seizoen_ibfk_1");
            });

            modelBuilder.Entity<Speler>(entity =>
            {
                entity.HasKey(e => e.Idspeler).HasName("PRIMARY");
                entity.ToTable("speler");
                entity.Property(e => e.Idspeler).HasColumnName("IDSpeler");
                entity.Property(e => e.Positie).HasMaxLength(10);
                entity.Property(e => e.SpelerAchternaam).HasMaxLength(50);
                entity.Property(e => e.SpelerVoornaam).HasMaxLength(50);
                entity.Property(e => e.Team).HasMaxLength(100).HasColumnName("Team");
            });

            modelBuilder.Entity<Wedstrijd>(entity =>
            {
                entity.HasKey(e => e.Idwedstrijd).HasName("PRIMARY");
                entity.ToTable("wedstrijd");
                entity.Property(e => e.Idwedstrijd).HasColumnName("IDWedstrijd");
                entity.Property(e => e.Score).HasMaxLength(10);
                entity.Property(e => e.ThuisTeam).HasMaxLength(50).HasColumnName("Thuis");
                entity.Property(e => e.UitTeam).HasMaxLength(50).HasColumnName("Uit");
                entity.Property(e => e.ThuisNaam).HasMaxLength(50).HasColumnName("ThuisNaam");
                entity.Property(e => e.UitNaam).HasMaxLength(50).HasColumnName("UitNaam");
            });

            modelBuilder.Entity<Wedstrijdheeftspeler>(entity =>
            {
                entity.HasKey(e => new { e.Fkspeler, e.Fkwedstrijd })
                      .HasName("PRIMARY")
                      .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("wedstrijdheeftspeler");
                entity.HasIndex(e => e.Fkwedstrijd, "FKWedstrijd");

                entity.Property(e => e.Fkspeler).HasColumnName("FKSpeler");
                entity.Property(e => e.Fkwedstrijd).HasColumnName("FKWedstrijd");

                entity.HasOne(d => d.FkspelerNavigation)
                      .WithMany(p => p.Wedstrijdheeftspelers)
                      .HasForeignKey(d => d.Fkspeler)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("wedstrijdheeftspeler_ibfk_1");

                entity.HasOne(d => d.FkwedstrijdNavigation)
                      .WithMany(p => p.Wedstrijdheeftspelers)
                      .HasForeignKey(d => d.Fkwedstrijd)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("wedstrijdheeftspeler_ibfk_2");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}