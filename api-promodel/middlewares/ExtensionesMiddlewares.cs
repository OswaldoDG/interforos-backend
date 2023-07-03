namespace api_promodel.middlewares
{
    public static class ExtensionesMiddlewares
    {
        public static IApplicationBuilder UseHostDetector(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HostDetectorMiddleware>();
        }
    }
}
