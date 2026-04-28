using APITask.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Lendo a string de conexão definida no appsettings.json
var sqlServerConnection = builder.Configuration.GetConnectionString("DefaultConnection"); 

// Registrando AppDbContext no container DI, informando o provider e a string de conexão
builder.Services.AddDbContext<AppDbContext>(options =>
                                            options.UseSqlServer(sqlServerConnection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
