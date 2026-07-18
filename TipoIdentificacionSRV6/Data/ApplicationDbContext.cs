using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
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
=======
using TipoIdentificacionSRV6.Entities;

namespace TipoIdentificacionSRV6.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TipoIdentificacion> TiposIdentificacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TipoIdentificacion>(entity =>
            {
                entity.ToTable("TIPOIDENTIFICACION", "PameRojas");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre)
                    .HasColumnName("NOMBRE")
                    .IsRequired()
                    .HasMaxLength(50);

                // Índice único para NOMBRE
                entity.HasIndex(e => e.Nombre).IsUnique();
            });
        }
>>>>>>> a7a79ac (Actualizacion del Login)
    }
}