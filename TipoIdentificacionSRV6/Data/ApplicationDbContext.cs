using Microsoft.EntityFrameworkCore;
using SRV6_TipoIdentificacion.Entities;

namespace SRV6_TipoIdentificacion.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TipoIdentificacion> TiposIdentificacion { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TipoIdentificacion>().ToTable("TIPOIDENTIFICACION", "PameRojas");

        modelBuilder.Entity<TipoIdentificacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Nombre).HasColumnName("NOMBRE");
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
        });
    }
}