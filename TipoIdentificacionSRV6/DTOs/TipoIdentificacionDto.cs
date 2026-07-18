<<<<<<< HEAD
﻿namespace SRV6_TipoIdentificacion.DTOs;

public class TipoIdentificacionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CrearTipoIdentificacionDto
{
    public string Nombre { get; set; } = string.Empty;
=======
﻿namespace TipoIdentificacionSRV6.DTOs
{
    public class TipoIdentificacionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class TipoIdentificacionCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class TipoIdentificacionUpdateDto
    {
        public int Id { get; set; }  // ✅ AGREGAR ESTA PROPIEDAD
        public string Nombre { get; set; } = string.Empty;
    }
>>>>>>> a7a79ac (Actualizacion del Login)
}