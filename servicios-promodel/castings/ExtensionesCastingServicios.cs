

using ImageMagick;
using Org.BouncyCastle.Asn1.X500;
using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using System;
using System.Text.Json;

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
            NombreUsuario=usuario.NombreUsuario,
        };
    }

    public static CastingListElement aCastingListElement(this Casting casting, TipoRolCliente rol)
    {
        return new CastingListElement()
        {
            Id = casting.Id,
            Nombre = casting.Nombre,
            NombreCliente = casting.NombreCliente,
            Status= casting.Status,
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


    public static SelectorCastingCategoria aSelectorCasting(this Casting casting, IServicioPersonas personasDb)
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
                c.Modelos = new List<ModeloOrdenable>();
                c.Comentarios = new List<ComentarioCategoriaModeloCasting>();
                c.Votos = new List<VotoModeloMapeo>();
                categoria.Modelos.ForEach(async m =>
                {
                    var r = personasDb.PorId(m.PersonaId).Result;
                   if(r.Ok)
                    {
                        var persona = (Persona)r.Payload;
                        string p = JsonSerializer.Serialize(persona);
                        ModeloOrdenable mo = JsonSerializer.Deserialize<ModeloOrdenable>(p);

                        mo.calificacion = m.CalificacionCalculada;

                        c.Modelos.Add(mo);
                    }
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
    public static int CalcularCalificaion(this List<VotoModeloCategoria> votos)
    {
        int total = 0;
        int MegustaMucho = 0;
        int Megusta = 0;
        int NomeGusta = 0;

        if (votos.Count>0)
        {
            votos.ForEach(voto=>
            {
            switch (voto.NivelLike) 
             {
                    case 0: 
                        {
                            NomeGusta += 1;
                            break;
                        }
                    case 2:
                        {
                            Megusta+= 1;
                            break;
                        }
                    case 3:
                        {
                            MegustaMucho+= 1;
                            break;
                        }
                }
            
            });
        }

        total = (int)(Math.Pow(MegustaMucho, 2) + Megusta - Math.Pow(NomeGusta, 2));
    return total;
    }


    public static  List<ModeloOrdenable> Ordenar( this SelectorCategoria categoria,string orden)
    {
       


        switch (orden)
        {
            case "mayor":
                {
                    return categoria.Modelos.OrderByDescending(_ => _.calificacion).ToList();                   
                }
            case "menor":
                {
                    return categoria.Modelos.OrderBy(_ => _.calificacion).ToList();
                }
            default:
                return categoria.Modelos.OrderBy(_ => _.NombreArtistico).ToList();
        }
        

    }

}

