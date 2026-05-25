using APITask.Data;
using APITask.Services;
using APITask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Lendo a string de conexão configurada para o ambiente atual
var sqlServerConnection = builder.Configuration.GetConnectionString("DefaultConnection"); 

// Registrando AppDbContext no container DI, informando o provider e a string de conexão
builder.Services.AddDbContext<AppDbContext>(options =>
                                            options.UseSqlServer(sqlServerConnection));

// Quando uma classe solicitar ITaskService, o container DI fornecerá uma instância de TaskService
builder.Services.AddScoped<ITaskService, TaskService>();

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
