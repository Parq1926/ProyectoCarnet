// Data/ApplicationDbContext.cs
using LoginSRV1.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoginSRV1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Sesion> Sesiones { get; set; }
        public DbSet<Parametro> Parametros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("USUARIO", "PameRojas");
            modelBuilder.Entity<Sesion>().ToTable("SESION", "PameRojas");
            modelBuilder.Entity<Parametro>().ToTable("PARAMETRO", "PameRojas");

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Email).HasColumnName("EMAIL").IsRequired();
                entity.Property(e => e.Contrasena).HasColumnName("CONTRASENA").IsRequired();
                entity.Property(e => e.TipoUsuarioId).HasColumnName("TIPO_USUARIO_ID").IsRequired();
                entity.Property(e => e.TipoIdentificacionId).HasColumnName("TIPO_IDENTIFICACION_ID").IsRequired();
                entity.Property(e => e.NumeroIdentificacion).HasColumnName("NUMERO_IDENTIFICACION").IsRequired();
                entity.Property(e => e.NombreCompleto).HasColumnName("NOMBRE_COMPLETO").IsRequired();
                entity.Property(e => e.Activo).HasColumnName("ACTIVO");

                // ✅ Manejar valores NULL y establecer valor por defecto para Bloqueado
                entity.Property(e => e.Bloqueado)
                    .HasColumnName("BLOQUEADO")
                    .HasDefaultValue(false);

                entity.Property(e => e.FechaCreacion).HasColumnName("FECHA_CREACION");
            });

            modelBuilder.Entity<Sesion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.UsuarioId).HasColumnName("USUARIO_ID");
                entity.Property(e => e.RefreshToken).HasColumnName("REFRESH_TOKEN").IsRequired();
                entity.Property(e => e.FechaExpiracion).HasColumnName("FECHA_EXPIRACION");
                entity.Property(e => e.Activo).HasColumnName("ACTIVO");
                entity.Property(e => e.FechaCreacion).HasColumnName("FECHA_CREACION");

                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Sesiones)
                      .HasForeignKey(e => e.UsuarioId);
            });

            modelBuilder.Entity<Parametro>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Valor).HasColumnName("VALOR").IsRequired();
                entity.Property(e => e.Descripcion).HasColumnName("DESCRIPCION");
            });
        }
    }
}