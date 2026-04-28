using APITask.Models;
using Microsoft.EntityFrameworkCore;

namespace APITask.Data;

public class AppDbContext : DbContext
{
    // Recebe as configurações de conexão definidas no Program.cs e repassa para a classe pai DbContext via base(options)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {    
    }

    public DbSet<TaskItem> Tasks { get; set; } = null!;
}
