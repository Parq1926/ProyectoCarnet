<<<<<<< HEAD
﻿namespace SRV5_TipoUsuario.DTOs;

public class TipoUsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CrearTipoUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
=======
﻿namespace TiposUsuarioSRV5.DTOs
{
    public class TipoUsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class TipoUsuarioCreateDto
    {
        public string Nombre { get; set; }
    }

    public class TipoUsuarioUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
>>>>>>> a7a79ac (Actualizacion del Login)
}