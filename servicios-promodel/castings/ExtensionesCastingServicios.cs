

using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace promodel.servicios.castings;

public static class ExtensionesCastingServicios
{
    public static ContactoCasting aContactoCasting(this ContactoUsuario usuario, DateTime? UltimoAcceso)
    {
        return new ContactoCasting()
        {
            Confirmado = false,
            Email = usuario.Email.ToLower(),
            Rol = (TipoRolCliente)usuario.Rol,
            UltimoIngreso = UltimoAcceso,
            UsuarioId = usuario.Id,
        };
    }

    public static CastingListElement aCastingListElement(this Casting casting, TipoRolCliente rol)
    {
        return new CastingListElement()
        {
            Id = casting.Id,
            Nombre = casting.Nombre,
            NombreCliente = casting.NombreCliente,
            FechaApertura = casting.FechaApertura,
            FechaCierre = casting.FechaCierre,
            AceptaAutoInscripcion = casting.AceptaAutoInscripcion,
            Activo = casting.Activo,
            AperturaAutomatica = casting.AperturaAutomatica,
            CierreAutomatico = casting.CierreAutomatico,
            Rol = rol
        };
    }
    public static CastingListElement aCastingListElement(this Casting casting)
    {
        return new CastingListElement()
        {
            Id = casting.Id,
            Nombre = casting.Nombre,
            NombreCliente = casting.NombreCliente,
            FechaApertura = casting.FechaApertura,
            FechaCierre = casting.FechaCierre,
            AceptaAutoInscripcion = casting.AceptaAutoInscripcion,
            Activo = casting.Activo,
            AperturaAutomatica = casting.AperturaAutomatica,
            CierreAutomatico = casting.CierreAutomatico,
        };
    }



    public static ComentarioCategoriaModeloCasting aComentarioCategoriaModeloCasting(this ComentarioCasting co,string categoriaId,string PersonaId)
    {
        return new ComentarioCategoriaModeloCasting()
        {
            Id = co.Id,
            Fecha = co.Fecha,
            UsuarioId = co.UsuarioId,
            Comentario = co.Comentario,
            CategoriaId = categoriaId,
            PersonaId = PersonaId,
        };

    }


    public static SelectorCastingCategoria aSelectorCasting(this Casting casting)
    {
        var selectorCastig = new SelectorCastingCategoria()
        {
            Id = casting.Id,
            Nombre = casting.Nombre,
            Categorias = new List<SelectorCategoria>(),
            Participantes= new List<MapaUsuarioNombre>()
    };

       if (casting.Categorias!=null)
        {
            casting.Categorias.ForEach(categoria =>
            {
                var c = new SelectorCategoria();
                c.Id = categoria.Id;
                c.Nombre = categoria.Nombre;
                c.Modelos = new List<string>();
                c.Comentarios = new List<ComentarioCategoriaModeloCasting>();
                c.Votos = new List<VotoModeloMapeo>();
                categoria.Modelos.ForEach(m =>
                {
                    c.Modelos.Add(m.PersonaId);

                    m.Comentarios.ForEach(co =>
                    {
                        var comentario = new ComentarioCategoriaModeloCasting()
                        {
                            Id = co.Id,
                            Fecha = co.Fecha,
                            UsuarioId = co.UsuarioId,
                            Comentario = co.Comentario,
                            CategoriaId = categoria.Id,
                            PersonaId = m.PersonaId,
                        };
                        c.Comentarios.Add(comentario);

                    });


                    m.Votos.ForEach(v =>
                    {
                        var voto = new VotoModeloMapeo()
                        {
                            NivelLike = v.NivelLike,
                            PersonaId = m.PersonaId,
                            UsuarioId = v.UsuarioId
                        };
                        c.Votos.Add(voto);
                    });

                });

                selectorCastig.Categorias.Add(c);
            });
        }
        return selectorCastig;
    }

}

