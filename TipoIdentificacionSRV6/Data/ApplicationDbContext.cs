using Microsoft.EntityFrameworkCore;
using TipoIdentificacionSRV6.Models;

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
                entity.ToTable("TIPOIDENTIFICACION");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Nombre).HasColumnName("NOMBRE");
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            });
        }
    }
}