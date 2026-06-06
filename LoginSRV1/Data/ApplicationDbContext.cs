using Microsoft.EntityFrameworkCore;
using LoginSRV1.Models;

namespace LoginSRV1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }
        public DbSet<TipoIdentificacion> TipoIdentificaciones { get; set; }
        public DbSet<Sesion> Sesiones { get; set; }
        public DbSet<Parametro> Parametros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar tabla USUARIO
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIO");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Email).HasColumnName("EMAIL");
                entity.Property(e => e.Contrasena).HasColumnName("CONTRASENA");
                entity.Property(e => e.TipoUsuarioId).HasColumnName("TIPO_USUARIO_ID");
                entity.Property(e => e.TipoIdentificacionId).HasColumnName("TIPO_IDENTIFICACION_ID");
                entity.Property(e => e.NumeroIdentificacion).HasColumnName("NUMERO_IDENTIFICACION");
                entity.Property(e => e.NombreCompleto).HasColumnName("NOMBRE_COMPLETO");
                entity.Property(e => e.Activo).HasColumnName("ACTIVO");
                entity.Property(e => e.FechaCreacion).HasColumnName("FECHA_CREACION");

                // Relaciones
                entity.HasOne(e => e.TipoUsuario)
                    .WithMany()
                    .HasForeignKey(e => e.TipoUsuarioId);

                entity.HasOne(e => e.TipoIdentificacion)
                    .WithMany()
                    .HasForeignKey(e => e.TipoIdentificacionId);
            });

            // Configurar tabla TIPOUSUARIO
            modelBuilder.Entity<TipoUsuario>(entity =>
            {
                entity.ToTable("TIPOUSUARIO");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Nombre).HasColumnName("NOMBRE");
            });

            // Configurar tabla TIPOIDENTIFICACION
            modelBuilder.Entity<TipoIdentificacion>(entity =>
            {
                entity.ToTable("TIPOIDENTIFICACION");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Nombre).HasColumnName("NOMBRE");
            });

            // Configurar tabla SESION
            modelBuilder.Entity<Sesion>(entity =>
            {
                entity.ToTable("SESION");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.UsuarioId).HasColumnName("USUARIO_ID");
                entity.Property(e => e.RefreshToken).HasColumnName("REFRESH_TOKEN");
                entity.Property(e => e.FechaExpiracion).HasColumnName("FECHA_EXPIRACION");
                entity.Property(e => e.Activo).HasColumnName("ACTIVO");
                entity.Property(e => e.FechaCreacion).HasColumnName("FECHA_CREACION");

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId);
            });

            // Configurar tabla PARAMETRO
            modelBuilder.Entity<Parametro>(entity =>
            {
                entity.ToTable("PARAMETRO");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.Valor).HasColumnName("VALOR");
                entity.Property(e => e.Descripcion).HasColumnName("DESCRIPCION");
            });
        }
    }
}