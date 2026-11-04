using Microsoft.OpenApi;

namespace ApiBet.OpenApi
{
    /// <summary>
    /// Extensiones para generar el documento OpenAPI y exponer Swagger UI.
    /// </summary>
    internal static class OpenApiExtensions
    {
        /// <summary>
        /// Registra la generación del documento OpenAPI de la API.
        /// </summary>
        /// <param name="services">Colección de servicios de la aplicación.</param>
        /// <returns>La misma colección para encadenar registros.</returns>
        public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Info ??= new OpenApiInfo();
                    document.Info.Title = "ApiBet";
                    document.Info.Version = "v1";
                    document.Info.Description = "HTTP API for synchronizing SharpAPI reference catalogs into SQL Server and querying the stored data.";
                    return Task.CompletedTask;
                });
            });

            return services;
        }

        /// <summary>
        /// Expone el documento OpenAPI y Swagger UI únicamente en Development.
        /// </summary>
        /// <param name="app">Aplicación web ya construida.</param>
        /// <returns>La misma aplicación para encadenar el pipeline.</returns>
        public static WebApplication UseApiDocumentation(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                return app;
            }

            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "ApiBet v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
                options.EnableTryItOutByDefault();
                options.EnableDeepLinking();
            });

            return app;
        }
    }
}
