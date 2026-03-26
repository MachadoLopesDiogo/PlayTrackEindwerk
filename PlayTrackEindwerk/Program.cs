using Microsoft.EntityFrameworkCore;
using PlayTrackEindwerk.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Server=localhost;Database=voetbaldb;User=root;Password=1234;";
builder.Services.AddDbContext<Voetbaldb>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// GET: profiel van de speler (eerste speler in db)
app.MapGet("/api/Speler", async (Voetbaldb db) =>
{
    var speler = await db.Spelers.FirstOrDefaultAsync();
    if (speler == null) return Results.NotFound();

    return Results.Ok(new
    {
        Id = speler.Idspeler,
        Voornaam = speler.SpelerVoornaam,
        Naam = speler.SpelerAchternaam,
        Positie = speler.Positie,
        Geboortedatum = (string?)null,
        Rugnummer = 1,
        Team = (string?)null
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

// GET: alle wedstrijden van de speler
app.MapGet("/api/Wedstrijd", async (Voetbaldb db) =>
{
    var speler = await db.Spelers.FirstOrDefaultAsync();
    if (speler == null) return Results.Ok(new List<object>());

    var lijst = await db.Wedstrijdheeftspelers
        .Where(w => w.Fkspeler == speler.Idspeler)
        .Include(w => w.FkwedstrijdNavigation)
        .Select(w => new
        {
            Id = w.Fkwedstrijd,
            Datum = w.FkwedstrijdNavigation.Datum.ToString(),
            ThuisTeam = w.FkwedstrijdNavigation.Score,
            UitTeam = "",
            ThuisScore = 0,
            UitScore = 0,
            IsThuis = true,
            Competitie = (string?)null,
            Goals = w.AantalGoals,
            Assists = w.AantalAssists,
            Rating = (decimal)w.RatingOp10
        })
        .ToListAsync();

    return Results.Ok(lijst);
});

// POST: wedstrijd toevoegen
app.MapPost("/api/Wedstrijd", async (WedstrijdRequest req, Voetbaldb db) =>
{
    var speler = await db.Spelers.FirstOrDefaultAsync();
    if (speler == null) return Results.BadRequest("Geen speler gevonden.");

    var wedstrijd = new Wedstrijd
    {
        Datum = DateOnly.Parse(req.Datum),
        Score = $"{req.ThuisScore}-{req.UitScore}"
    };
    db.Wedstrijds.Add(wedstrijd);
    await db.SaveChangesAsync();

    var stat = new Wedstrijdheeftspeler
    {
        Fkspeler = speler.Idspeler,
        Fkwedstrijd = wedstrijd.Idwedstrijd,
        AantalGoals = req.Goals,
        AantalAssists = req.Assists,
        AantalGeleRodeKaarten = req.Geel + req.Rood,
        AantalGrofFouten = req.Fouten,
        AantalBelangrijkeActies = req.Aanvallen,
        AantalBelangrijkeTackles = req.Tackles,
        RatingOp10 = 5
    };
    db.Wedstrijdheeftspelers.Add(stat);
    await db.SaveChangesAsync();

    return Results.Ok();
});

// GET: seizoensstatistieken
app.MapGet("/api/Seizoen/stats", async (Voetbaldb db) =>
{
    var speler = await db.Spelers.FirstOrDefaultAsync();
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

app.Run();

// DTOs
record SpelerDto
 (string Voornaam, string Naam, string? Geboortedatum, string Positie, int Rugnummer, string? Team);

record WedstrijdRequest(string Datum, string? Competitie, string ThuisTeam, string UitTeam,
    int ThuisScore, int UitScore, bool IsThuis,
    int Goals, int Assists, int Saves, int Tackles, int Aanvallen, int Fouten, int Geel, int Rood);