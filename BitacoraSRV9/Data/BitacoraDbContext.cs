using BitacoraSRV9.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitacoraSRV9.Data;

public class BitacoraDbContext : DbContext
{
    public BitacoraDbContext(
        DbContextOptions<BitacoraDbContext> options)
        : base(options)
    {
    }

    public DbSet<Bitacora> Bitacoras { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bitacora>()
            .ToTable("BITACORA");

        modelBuilder.Entity<Bitacora>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Bitacora>()
            .Property(x => x.Id)
            .HasColumnName("ID");

        modelBuilder.Entity<Bitacora>()
            .Property(x => x.Usuario)
            .HasColumnName("USUARIO");

        modelBuilder.Entity<Bitacora>()
            .Property(x => x.Accion)
            .HasColumnName("ACCION");

        modelBuilder.Entity<Bitacora>()
            .Property(x => x.Fecha)
            .HasColumnName("FECHA");
    }
}