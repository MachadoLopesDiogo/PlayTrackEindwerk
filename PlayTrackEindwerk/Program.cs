using Microsoft.EntityFrameworkCore;
using PlayTrackEindwerk.Models;
using System.Collections.Generic;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=localhost;Database=voetbaldb;User=root;Password=1234;";

builder.Services.AddDbContext<Voetbaldb>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ✅ Slechts één keer
app.UseHttpsRedirection();
app.UseCors();
app.UseSession();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




// GET: profiel van de speler
app.MapGet("/api/Speler", async (Voetbaldb db) =>
{
    try
    {
        Console.WriteLine("🔄 Proberen speler op te halen...");

        var speler = await db.Spelers.FirstOrDefaultAsync();

        if (speler == null)
        {
            Console.WriteLine("⚠️ Geen speler gevonden in de database.");
            return Results.NotFound("Geen speler gevonden");
        }

        Console.WriteLine($"✅ Speler succesvol gevonden: {speler.SpelerVoornaam} {speler.SpelerAchternaam}");

        return Results.Ok(new
        {
            Id = speler.Idspeler,
            Voornaam = speler.SpelerVoornaam,
            Naam = speler.SpelerAchternaam,
            Positie = speler.Positie ?? "Onbekend",
            Geboortedatum = (string?)null,
            Rugnummer = 1,
            Team = (string?)null
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine("🚨 KRITIEKE FOUT in /api/Speler:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.ToString());
        return Results.Problem("Database fout: " + ex.Message, statusCode: 500);
    }
});


app.MapGet("/api/Speler/{id}", async (int id, Voetbaldb db) =>
{
    var speler = await db.Spelers.FindAsync(id);
    if (speler == null) return Results.NotFound();
    return Results.Ok(new
    {
        Id = speler.Idspeler,
        Voornaam = speler.SpelerVoornaam,
        Naam = speler.SpelerAchternaam,
        Positie = speler.Positie,
        Team = speler.Team
    });
});




// POST: profiel opslaan
app.MapPost("/api/Speler", async (SpelerDto dto, Voetbaldb db) =>
{
    var speler = await db.Spelers.FirstOrDefaultAsync();
    if (speler == null)
    {
        speler = new Speler
        {
            SpelerVoornaam = dto.Voornaam,
            SpelerAchternaam = dto.Naam,
            Positie = dto.Positie
        };
        db.Spelers.Add(speler);
    }
    else
    {
        speler.SpelerVoornaam = dto.Voornaam;
        speler.SpelerAchternaam = dto.Naam;
        speler.Positie = dto.Positie;
    }

    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/api/Wedstrijd/{spelerId}", async (int spelerId, Voetbaldb db) =>
{
    var lijst = await db.Wedstrijdheeftspelers
        .Where(w => w.Fkspeler == spelerId)
        .Include(w => w.FkwedstrijdNavigation)
        .OrderByDescending(w => w.FkwedstrijdNavigation.Datum)
        .ToListAsync();

    var speler = await db.Spelers.FindAsync(spelerId);
    var teamNaam = speler?.Team ?? "";

    var result = lijst.Select(w =>
    {
        var delen = w.FkwedstrijdNavigation.Score?.Split('-') ?? new[] { "0", "0" };
        int thuisScore = int.TryParse(delen[0], out var t) ? t : 0;
        int uitScore = delen.Length > 1 && int.TryParse(delen[1], out var u) ? u : 0;

        bool isThuis = string.Equals(w.FkwedstrijdNavigation.ThuisNaam, teamNaam, StringComparison.OrdinalIgnoreCase);
        if (!isThuis && !string.Equals(w.FkwedstrijdNavigation.UitNaam, teamNaam, StringComparison.OrdinalIgnoreCase))
            isThuis = true;

        int mijnScore = isThuis ? thuisScore : uitScore;
        int tegensScore = isThuis ? uitScore : thuisScore;

        string resultaat = mijnScore > tegensScore ? "Gewonnen"
                         : mijnScore == tegensScore ? "Gelijk"
                         : "Verloren";
        return new
        {
            Id = w.Fkwedstrijd,
            Datum = w.FkwedstrijdNavigation.Datum.ToString("yyyy-MM-dd"),
            ThuisTeam = w.FkwedstrijdNavigation.ThuisNaam,
            UitTeam = w.FkwedstrijdNavigation.UitNaam,
            ThuisScore = thuisScore,
            UitScore = uitScore,
            IsThuis = isThuis,
            Resultaat = resultaat,
            Goals = w.AantalGoals,
            Assists = w.AantalAssists,
            Rating = (decimal)w.RatingOp10
        };
    }).ToList();

    return Results.Ok(result);
});
app.MapPost("/api/Wedstrijd", async (WedstrijdRequest req, Voetbaldb db) =>
{
    var speler = await db.Spelers.FindAsync(req.SpelerId);
    if (speler == null) return Results.BadRequest("Geen speler gevonden.");

    var wedstrijd = new Wedstrijd
    {
        Datum = DateOnly.Parse(req.Datum),
        Score = $"{req.ThuisScore}-{req.UitScore}",
        ThuisTeam = req.ThuisTeam ?? "10",
        UitTeam = req.UitTeam ?? "1",
        ThuisNaam = string.IsNullOrWhiteSpace(req.ThuisTeam) ? "Olen United" : req.ThuisTeam,
        UitNaam = string.IsNullOrWhiteSpace(req.UitTeam) ? "Tegenstander" : req.UitTeam
    };
    db.Wedstrijds.Add(wedstrijd);
    await db.SaveChangesAsync();

    db.Wedstrijdheeftspelers.Add(new Wedstrijdheeftspeler
    {
        Fkspeler = speler.Idspeler,
        Fkwedstrijd = wedstrijd.Idwedstrijd,
        AantalGoals = req.Goals,
        AantalAssists = req.Assists,
        AantalGeleRodeKaarten = req.Geel + req.Rood,
        AantalGrofFouten = req.Fouten,
        AantalBelangrijkeActies = req.Aanvallen,
        AantalBelangrijkeTackles = req.Tackles,

       


        RatingOp10 = (int)BerekenRating(req.Goals, req.Assists, req.Tackles, req.Aanvallen, req.Fouten, req.Geel, req.Rood, req.Gewonnen,req.Saves)


    });
    await db.SaveChangesAsync();
    return Results.Ok(new { Success = true });
});
// GET: seizoensstatistieken
app.MapGet("/api/Seizoen/stats", async (int spelerId, Voetbaldb db) =>
{
    var speler = await db.Spelers.FindAsync(spelerId);
    if (speler == null)
        return Results.Ok(new { Wedstrijden = 0, Goals = 0, Assists = 0, Rating = 0m, Gewonnen = 0, Gelijk = 0, Verloren = 0 });

    var stats = await db.Wedstrijdheeftspelers
        .Where(w => w.Fkspeler == speler.Idspeler)
        .Include(w => w.FkwedstrijdNavigation)
        .ToListAsync();

    int gewonnen = 0, gelijk = 0, verloren = 0;
    foreach (var s in stats)
    {
        var delen = s.FkwedstrijdNavigation.Score.Split('-');
        if (delen.Length == 2 && int.TryParse(delen[0], out int t) && int.TryParse(delen[1], out int u))
        {
            if (t > u) gewonnen++;
            else if (t == u) gelijk++;
            else verloren++;
        }
    }

    return Results.Ok(new
    {
        Wedstrijden = stats.Count,
        Goals = stats.Sum(s => s.AantalGoals),
        Assists = stats.Sum(s => s.AantalAssists),
        Rating = stats.Count > 0 ? (decimal)stats.Average(s => s.RatingOp10) : 0m,
        Gewonnen = gewonnen,
        Gelijk = gelijk,
        Verloren = verloren
    });
});
app.MapPost("/api/Login", async (LoginDto dto, Voetbaldb db) =>
{
    var speler = await db.Spelers.FirstOrDefaultAsync(s =>
        s.SpelerVoornaam.ToLower().Trim() == dto.Voornaam.ToLower().Trim() &&
        s.SpelerAchternaam.ToLower().Trim() == dto.Achternaam.ToLower().Trim());

    if (speler == null)
        return Results.NotFound(new { bericht = "Speler niet gevonden." });

    return Results.Ok(new
    {
        Id = speler.Idspeler,
        Voornaam = speler.SpelerVoornaam,
        Achternaam = speler.SpelerAchternaam,
        Positie = speler.Positie,
        Team = speler.Team   
    });
});
app.MapPost("/api/Register", async (SpelerDto dto, Voetbaldb db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Team))
        return Results.BadRequest(new { bericht = "Teamnaam is verplicht." });

    var nieuw = new Speler
    {
        SpelerVoornaam = dto.Voornaam.Trim(),
        SpelerAchternaam = dto.Naam.Trim(),
        Positie = dto.Positie?.Trim() ?? "",
        Team = dto.Team.Trim()   
    };
    db.Spelers.Add(nieuw);
    await db.SaveChangesAsync();
    return Results.Ok(new { Id = nieuw.Idspeler });
});
static double BerekenRating(int goals, int assists, int tackles, int aanvallen, int fouten, int geel, int rood, bool gewonnen,int saves)
{
    double rating = 5.0;
    rating += goals * 1.5;
    rating += assists * 1.2 ;
    rating += tackles * 0.3;
    rating += aanvallen * 0.2;
    rating += saves * 0.7;
    rating -= fouten * 0.3;
    rating -= geel * 0.5;
    rating -= rood * 1.5;

    if (gewonnen) rating += 1.0;
    return Math.Clamp(rating, 1.0, 10.0);
}
app.Run();

// DTOs
record SpelerDto
 (string Voornaam, string Naam, string? Geboortedatum, string Positie, int Rugnummer, string? Team);

record WedstrijdRequest(int SpelerId, string Datum, string? Competitie, string ThuisTeam, string UitTeam,
    int ThuisScore, int UitScore, bool IsThuis,
    int Goals, int Assists, int Saves, int Tackles, int Aanvallen, int Fouten, int Geel, int Rood, bool Gewonnen);
record LoginDto(string Voornaam, string Achternaam);

