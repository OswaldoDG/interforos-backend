﻿using api_promodel.middlewares;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo;
using promodel.modelo.clientes;
using promodel.modelo.controllers;
using promodel.modelo.perfil;
using promodel.servicios;
using System.IdentityModel.Tokens.Jwt;

namespace api_promodel.Controllers;


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
    private readonly IServicioIdentidad identidad;
    private readonly IServicioPersonas servicioPersonas;

    public ControllerUsoInterno(IServicioClientes servicioClientes, IServicioIdentidad servicioIdentidad)
    {
        this.servicioClientes = servicioClientes;
        identidad = servicioIdentidad;
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


    protected  TipoRolCliente RolUsuario
    {
        get
        {
                try
                {

                    if (identidad != null && this.UsuarioId!=null)
                    {
                        var user = identidad.UsuarioPorId(this.UsuarioId).Result;
                        var rol = user.RolesCliente.FirstOrDefault(_ => _.ClienteId == this.ClienteId);
                        return rol.Rol;
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }

            return TipoRolCliente.Ninguno;
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
