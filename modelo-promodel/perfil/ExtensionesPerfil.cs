namespace promodel.modelo.perfil
{
    public static class ExtensionesPerfil
    {
        public static int DiferenciaAnos(this DateTime? fecha, DateTime? actual = null)
        {
            if (!fecha.HasValue)
            {
                return 0;
            }

            if (actual == null)
            {
                actual = DateTime.UtcNow;
            }
            TimeSpan TS = actual.Value - fecha.Value;
            double Years = TS.TotalDays / 365.25;

            return (int)Years;
        }

        public static PersonaView ToPersonaView(this Persona p )
        {
            PersonaView v = new PersonaView()
            {
                ActividadesIds = p.ActividadesIds,
                Apellido1 = p.Apellido1,
                Apellido2 = p.Apellido2,
                ElementoMedioPrincipalId = p.ElementoMedioPrincipalId,
                EstadoPaisId = p.EstadoPaisId,
                Exclusivo = p.Exclusivo,
                Extranjero = p.Extranjero,
                FechaNacimiento = p.FechaNacimiento,
                GeneroId = p.GeneroId,
                IdiomasIds = p.IdiomasIds,
                Nombre = p.Nombre,
                NombreArtistico = p.NombreArtistico,
                OffsetHorario = p.OffsetHorario,
                PaisOrigenId = p.PaisOrigenId,
                PermisoTrabajo = p.PermisoTrabajo,
                PropiedadesFisicas = p.PropiedadesFisicas,
                PropiedadesVestuario = p.PropiedadesVestuario,
                Relacion = p.Relacion,
                UsuarioId = p.UsuarioId,
                ZonaHorariaId = p.ZonaHorariaId,
                Contacto = p.Contacto
            };

            return v;
        }
    }
}
