using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace PlayTrackEindwerk.Models;

public partial class Voetbaldb : DbContext
{
    public Voetbaldb()
    {
    }

    public Voetbaldb(DbContextOptions<Voetbaldb> options)
        : base(options)
    {
    }

    public virtual DbSet<Seizoen> Seizoens { get; set; }

    public virtual DbSet<Speler> Spelers { get; set; }

    public virtual DbSet<Wedstrijd> Wedstrijds { get; set; }

    public virtual DbSet<Wedstrijdheeftspeler> Wedstrijdheeftspelers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseMySql("server=localhost;database=voetbaldb;user=root;password=1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));

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

            entity.HasOne(d => d.FkwedstrijdNavigation).WithMany(p => p.Seizoens)
                .HasForeignKey(d => d.Fkwedstrijd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seizoen_ibfk_1");
        });

        modelBuilder.Entity<Speler>(entity =>
        {
            entity.HasKey(e => e.Idspeler).HasName("PRIMARY");

            entity.ToTable("speler");

            entity.Property(e => e.Idspeler).HasColumnName("IDSpeler");
            entity.Property(e => e.Positie).HasMaxLength(10);
            entity.Property(e => e.SpelerAchternaam).HasMaxLength(50);
            entity.Property(e => e.SpelerVoornaam).HasMaxLength(50);
        });

        modelBuilder.Entity<Wedstrijd>(entity =>
        {
            entity.HasKey(e => e.Idwedstrijd).HasName("PRIMARY");

            entity.ToTable("wedstrijd");

            entity.Property(e => e.Idwedstrijd).HasColumnName("IDWedstrijd");
            entity.Property(e => e.Score).HasMaxLength(10);
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

            entity.HasOne(d => d.FkspelerNavigation).WithMany(p => p.Wedstrijdheeftspelers)
                .HasForeignKey(d => d.Fkspeler)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("wedstrijdheeftspeler_ibfk_1");

            entity.HasOne(d => d.FkwedstrijdNavigation).WithMany(p => p.Wedstrijdheeftspelers)
                .HasForeignKey(d => d.Fkwedstrijd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("wedstrijdheeftspeler_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
