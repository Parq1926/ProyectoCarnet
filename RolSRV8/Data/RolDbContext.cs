using Microsoft.EntityFrameworkCore;
using RolSRV8.Entities;

namespace RolSRV8.Data;

public class RolDbContext : DbContext
{
    public RolDbContext(
        DbContextOptions<RolDbContext> options)
        : base(options)
    {
    }

    public DbSet<Rol> Roles { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>()
            .ToTable("ROL");

        modelBuilder.Entity<Rol>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Rol>()
            .Property(x => x.Id)
            .HasColumnName("ID");

        modelBuilder.Entity<Rol>()
            .Property(x => x.Nombre)
            .HasColumnName("NOMBRE");

        modelBuilder.Entity<Rol>()
            .Property(x => x.Pantallas)
            .HasColumnName("PANTALLAS");
    }
}