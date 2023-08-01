using promodel.modelo;
using CouchDB.Driver.Extensions;
using CouchDB.Driver.Query.Extensions;
using comunicaciones.email;
using Microsoft.Extensions.Configuration;
using promodel.servicios.identidad;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using promodel.modelo.registro;

namespace promodel.servicios
{
    public partial class ServicioIdentidad : IServicioIdentidad
    {

        /// <summary>
        /// Realiza la solicitud de registro de un usuario
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public async Task Registro(RegistroUsuario r)
        {
            InvitacionRegistro inv = await db.Invitaciones.Where(x => x.Registro.Email == r.Email && x.Registro.ClienteId == r.ClienteId)
                .UseIndex(new[] { "design_document", IdentidadCouchDbContext.IDX_INVITACION }).FirstOrDefaultAsync();

            if (inv == null)
            {
                inv = new() { Emitida = DateTime.UtcNow, LimiteUso = DateTime.UtcNow.AddDays(1), Registro = r, Id = Guid.NewGuid().ToString() };

            }
            else
            {
                inv.LimiteUso = DateTime.UtcNow.AddDays(1);
            }

            await db.Invitaciones.AddOrUpdateAsync(inv);



            DatosPlantillaRegistro data = new() { 
                Activacion = inv.Id, 
                FechaLimite = inv.LimiteUso.ToString(), 
                Nombre = inv.Registro.Nombre, 
                UrlBase = configuration.LeeUrlBase(), 
                Remitente = inv.Registro.Nombre,
                Rol = RolComoTexto(inv.Registro.Rol)
            };

            MensajeEmail m = new()
            {
                DireccionPara = r.Email,
                NombrePara = r.Nombre,
                JSONData = JsonConvert.SerializeObject(data),
                PlantillaCuerpo = configuration.LeePlantillaRegistro(environment, inv),
                PlantillaTema = configuration.LeeTemaRegistro(inv)
            };

            await servicioEmail.Enviar(m);
        }

        private string RolComoTexto(TipoRolCliente rol)
        {
            switch (rol)
            {
                case TipoRolCliente.Staff:
                    return "Miembro del equipo";

                case TipoRolCliente.RevisorExterno:
                    return "Revisor del casting";

                default:
                    return "";
            }
        }

        public async Task<InvitacionRegistro?> RegistroPorId(string Id)
        {
            InvitacionRegistro? inv = await db.Invitaciones.Where(x => x.Id == Id).FirstOrDefaultAsync();

            return inv;
        }

        public async Task EliminaRegistroPorId(string Id)
        {
            var i = await db.Invitaciones.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (i != null)
            {
                await db.Invitaciones.RemoveAsync(i);
            }
        }
    }
}