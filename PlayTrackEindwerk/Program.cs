using Microsoft.EntityFrameworkCore;
using PlayTrackEindwerk.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Server=localhost;Database=voetbaldb;User=root;Password=1234;";
builder.Services.AddDbContext<Voetbaldb>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
