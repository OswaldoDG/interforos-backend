using System.Text;

namespace promodel.modelo
{
    public static class ExtensionesUsuario
    {
        public static InvitacionRegistro? Sanitiza(this InvitacionRegistro r)
        {
            if(r!=null)
            {
                r.Registro.ClienteId = null;
            }

            return r;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static Usuario? AUsuario(this  InvitacionRegistro r)
        {
            if (r != null)
            {
                Usuario u = new()
                {
                    Activo = true,
                    Bloqueado = false,
                    Clientes = new List<string>() { r.Registro.ClienteId },
                    RolesCliente = new List<registro.RolCliente>() { new registro.RolCliente() { ClienteId = r.Registro.ClienteId, Rol = r.Registro.Rol } },
                    Email = r.Registro.Email.ToLower(),
                    HashContrasena = null,
                    Id = Guid.NewGuid().ToString(),
                    NombreAcceso = r.Registro.Email.ToLower(),
                    AgenciaId = r.Registro.AgenciaId
                };

                return u;
            }

            return null;
        }

    }
}
