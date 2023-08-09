

using System.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using A = DocumentFormat.OpenXml.Drawing;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using System.Globalization;
using Google.Apis.Json;
using DocumentFormat.OpenXml.Bibliography;

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

    public static SelectorCastingCategoria aSelectorCasting(this Casting casting)
    {
        var selectorCastig = new SelectorCastingCategoria()
        {
            Id = casting.Id,
            Nombre = casting.Nombre,
            Categorias = new List<SelectorCategoria>()
        };

        casting.Categorias.ForEach(categoria =>
        {
            selectorCastig.Categorias.Add(new SelectorCategoria()
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Modelos = categoria.Modelos.Select(_ => _.PersonaId).ToList(),
            }); ;
        });
        return selectorCastig;
    }
}

