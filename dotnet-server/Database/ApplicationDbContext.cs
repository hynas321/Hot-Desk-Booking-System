using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {}

    public ApplicationDbContext() {}

    public DbSet<User>? Users { get; set; }
    public DbSet<Location>? Locations { get; set; }
    public DbSet<Desk>? Desks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       optionsBuilder.UseSqlite("Data Source=app.db");
       optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Desk>().HasKey(d => d.Id);
        modelBuilder.Entity<Location>().HasKey(l => l.LocationName);

        modelBuilder.Entity<Location>()
            .HasMany(l => l.Desks)
            .WithOne(d => d.Location);
    }
}