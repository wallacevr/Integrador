using Microsoft.EntityFrameworkCore;
namespace HelpIn;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Ong> Ongs { get; set; } // Ong é sua model

    public DbSet<Voluntario> Voluntarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Garante que Email é único, se não estiver usando [Index]
        modelBuilder.Entity<Ong>().HasIndex(o => o.Email).IsUnique();
        modelBuilder.Entity<Voluntario>().HasIndex(v => v.Email).IsUnique();
    }
}
