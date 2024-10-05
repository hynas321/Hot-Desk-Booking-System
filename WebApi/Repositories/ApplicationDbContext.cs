using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Repositories;

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
       optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
       optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Desk>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Desk>()
            .HasOne(d => d.Location)
            .WithMany(l => l.Desks)
            .HasForeignKey(d => d.LocationId);

        modelBuilder.Entity<Location>()
            .HasKey(l => l.Id);

        modelBuilder.Entity<Booking>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Desk)
            .WithMany(d => d.Bookings)
            .HasForeignKey(b => b.DeskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}