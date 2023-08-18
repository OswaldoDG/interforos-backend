using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace api_promodel.middlewares
{
    public class ControladorAutenticadoFilter : IActionFilter
    {
        private const string HCLIENTEID = "clienteid";

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // if (string.IsNullOrEmpty(ClienteId(context)) || string.IsNullOrEmpty(UsuarioId(context)))
            if (string.IsNullOrEmpty(ClienteId(context)))
            {
                context.Result = new BadRequestResult();
                return;
            }
        }

        private string? ClienteId(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.Any(x => x.Key == HCLIENTEID))
            {
                return context.HttpContext.Request.Headers[HCLIENTEID].First();
            }
            return null;
        }

        private string? UsuarioId(ActionExecutingContext context)
        {
            try
            {
                string jwt = context.HttpContext.Request.Headers["Authorization"];
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
}
