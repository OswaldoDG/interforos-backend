﻿using promodel.modelo.castings;
using promodel.modelo.proyectos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios.proyectos
{
    public interface ICastingService
    {

        Task<RespuestaPayload<CastingListElement>> Casting(string ClienteId, bool incluirInactivos);
        Task<Respuesta> EliminarCasting(string ClienteId, string CastingId, string UsuarioId);
        Task<Respuesta> EstadoCasting(string ClienteId, string CastingId, string UsuarioId, bool Activo);
        Task<RespuestaPayload<Casting>> CreaCasting(string ClienteId, string UsuarioId, Casting casting);
        Task<Respuesta> ActualizaCasting(string ClienteId, string UsuarioId, string CastingId, Casting casting);
        Task<RespuestaPayload<Casting>> FullCasting(string ClienteId, string CastingId, string UsuarioId);

        Task<Respuesta> EliminarCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId);
        Task<RespuestaPayload<CategoriaCasting>> CrearCategoria(string ClienteId, string CastingId, string UsuarioId, CategoriaCasting categoria);
        Task<RespuestaPayload<CategoriaCasting>> ActualizarCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoriaId, CategoriaCasting categoria);

        Task<Respuesta> EliminarModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, OrigenInscripcion origen);
        Task<Respuesta> AdicionarModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, OrigenInscripcion origen);

        Task<Respuesta> EliminarComentarioModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, string ComentarioId);
        Task<RespuestaPayload<ComentarioCasting>> AdicionarComentarioModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, string Comentario);

        Task<Respuesta> EliminarComentarioCasting(string ClienteId, string CastingId, string UsuarioId, string ComentarioId);
        Task<RespuestaPayload<ComentarioCasting>> AdicionarComentarioCasting(string ClienteId, string CastingId, string UsuarioId, string comentario);
        
        Task<Respuesta> EliminarStaff(string ClienteId, string CastingId, string? UsuarioId, string? Email);
        
        Task<RespuestaPayload<StaffCasting>> AdicionarStaff(string ClienteId, string CastingId, string? UsuarioId, string? email);

        Task<Respuesta> AdicionarColaboradoresCasting(string ClienteId, string CastingId, string UsuarioId, List<string> ColaboradoresIds);
        Task<Respuesta> RemoverColaboradoresCasting(string ClienteId, string CastingId, string UsuarioId, List<string> ColaboradoresIds);
    }
}