using promodel.servicios;
using System.Net;

namespace api_promodel.middlewares
{
    public class HostDetectorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServicioClientes servicioClientes;
        private const string HCLIENTEID = "clienteid";
        public HostDetectorMiddleware(RequestDelegate next, IServicioClientes servicioClientes)
        {
            _next = next;
            this.servicioClientes = servicioClientes;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? origin = context.Request.Headers["Origin"];
            if(origin == null)
            {
                origin = context.Request.Headers["Referer"];
            }
            if (!string.IsNullOrEmpty(origin))
            {
                origin = origin.TrimEnd('/');
            }
            origin ??= "https://localhost";
            bool deny = true;

            var c = await servicioClientes.ClientePorUrl(origin);
            if(c!=null)
            {
                if (c.Activo)
                {
                    deny  = false;
                    if(context.Request.Headers.Any(x=>x.Key == HCLIENTEID))
                    {
                        context.Request.Headers.Remove(HCLIENTEID);
                    }

                    context.Request.Headers.Add(HCLIENTEID, c.Id);
                    // Call the next delegate/middleware in the pipeline.
                    await _next(context);
                }
            } 

            if(deny)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
            }
            
        }

    }
}
