using api_promodel.middlewares;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo.clientes;
using promodel.modelo.controllers;
using promodel.servicios;
using System.IdentityModel.Tokens.Jwt;

namespace api_promodel.Controllers
{
    
    public class ControllerUsoInterno : ControllerBase, IControladorCliente
    {

        public string? ClienteId
        {
            get
            {
                return this.Request.Headers["clienteid"];
            }
        }

        public readonly IServicioClientes servicioClientes;
        public ControllerUsoInterno(IServicioClientes servicioClientes)
        {
            this.servicioClientes = servicioClientes;
        }

        [NonAction]
        public ActionResult ActionFromCode(promodel.servicios.HttpCode code, string? Error = null)
        {
            switch (code)
            {
                case promodel.servicios.HttpCode.Conflict:
                    if (string.IsNullOrWhiteSpace(Error))
                    {
                        return Conflict();
                    }
                    else
                    {
                        return Conflict(Error);
                    }


                case promodel.servicios.HttpCode.NotFound:
                    if (string.IsNullOrWhiteSpace(Error))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return NotFound(Error);
                    }


                case promodel.servicios.HttpCode.BadRequest:
                    if (string.IsNullOrWhiteSpace(Error))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        return BadRequest(Error);
                    }

            }

            if (string.IsNullOrWhiteSpace(Error))
            {
                return BadRequest();
            }
            else
            {
                return BadRequest(Error);
            }

        }

        public string? UsuarioId
        {
            get
            {
                try
                {
                    string jwt = this.Request.Headers["Authorization"];
                    if (!string.IsNullOrEmpty(jwt) && jwt.IndexOf("Bearer", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jsonToken = handler.ReadToken(jwt.Split(' ')[1]);
                        var tokenS = jsonToken as JwtSecurityToken;
                        return tokenS?.Subject;
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }

                return null;
            }
        }

        [NonAction]
        public async Task<Cliente?> Cliente()
        {

            var c = await servicioClientes.ClientePorId(ClienteId);
            return c;
        }

    }
}
