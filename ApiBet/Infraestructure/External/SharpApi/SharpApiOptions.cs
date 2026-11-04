namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Opciones de configuración para el cliente HTTP de SharpAPI.
    /// </summary>
    public sealed class SharpApiOptions
    {
        /// <summary>
        /// Nombre de la sección en appsettings.
        /// </summary>
        public const string SectionName = "SharpApi";

        /// <summary>
        /// URL base de SharpAPI, incluyendo la versión.
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.sharpapi.io/api/v1/";

        /// <summary>
        /// Clave privada enviada en el encabezado X-API-Key.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}
