﻿using Microsoft.AspNetCore.Mvc;
using promodel.modelo.clientes;
using promodel.modelo.controllers;
using promodel.modelo.perfil;
using promodel.servicios;
using System.IdentityModel.Tokens.Jwt;
using System.IO;

namespace api_promodel.Controllers.publico
{
    public class ControllerPublico : ControllerBase, IControladorCliente
    {
        private readonly IServicioClientes servicioClientes;
        private readonly IServicioPersonas servicioPersonas;

        public ControllerPublico(IServicioClientes servicioClientes, IServicioPersonas servicioPersonas)
        {
            this.servicioClientes = servicioClientes;
            this.servicioPersonas = servicioPersonas;
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
                    } else
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


        public string? ClienteId
        {
            get
            {
                return this.Request.Headers["clienteid"];
            }
        }

        public string? UsuarioId
        {
            get
            {
                try
                {
                    string jwt = this.Request.Headers["Authorization"];
                    if (!string.IsNullOrEmpty(jwt) && jwt.IndexOf("Bearer", StringComparison.InvariantCultureIgnoreCase)>=0)
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

        public  string? PersonaId
        {
            get
            {
                try
                {
                    var usuarioId = this.UsuarioId;
                    if (!string.IsNullOrEmpty(usuarioId))
                    {
                      var respuesta = servicioPersonas.PorUsuarioId(usuarioId).Result;
                        
                        if(respuesta.Ok)
                        {
                            var persona = (Persona)respuesta.Payload;
                            return persona.Id;
                        }

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

        public ControllerPublico()
        {

        }

    }
}
