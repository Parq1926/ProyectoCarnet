<<<<<<< HEAD
﻿namespace SRV5_TipoUsuario.Entities;

public class TipoUsuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
=======
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiposUsuarioSRV5.Entities
{
    [Table("TIPOUSUARIO", Schema = "PameRojas")]
    public class TipoUsuario
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("NOMBRE")]
        public string Nombre { get; set; }
    }
>>>>>>> a7a79ac (Actualizacion del Login)
}