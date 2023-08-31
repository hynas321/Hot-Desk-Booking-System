using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration configuration;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        this.configuration = configuration;
    }

    public ApplicationDbContext(IConfiguration configuration) 
    {
        this.configuration = configuration;
    }

    public DbSet<User>? Users { get; set; }
    public DbSet<Location>? Locations { get; set; }
    public DbSet<Desk>? Desks { get; set; }
    public DbSet<Booking>? Bookings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
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