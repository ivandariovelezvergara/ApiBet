using System.Text.Json;
using Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace Infraestructure.External.SharpApi
{
    /// <summary>
    /// Puerta de acceso HTTP compartida hacia SharpAPI.
    /// </summary>
    internal sealed class SharpApiHttpGateway
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Inicializa la puerta de acceso con el cliente HTTP configurado.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP con URL base y encabezado X-API-Key.</param>
        /// <param name="options">Opciones de SharpAPI usadas para validar la configuración.</param>
        public SharpApiHttpGateway(HttpClient httpClient, IOptions<SharpApiOptions> options)
        {
            _httpClient = httpClient;

            if (string.IsNullOrWhiteSpace(options.Value.ApiKey))
            {
                throw new InvalidOperationException("SharpApi:ApiKey is required.");
            }
        }

        /// <summary>
        /// Ejecuta un GET JSON contra SharpAPI y deserializa la respuesta.
        /// </summary>
        /// <typeparam name="TResponse">Contrato JSON esperado.</typeparam>
        /// <param name="relativePath">Ruta relativa al BaseUrl (por ejemplo, leagues).</param>
        /// <param name="resourceName">Nombre del recurso, usado en mensajes de error.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Payload deserializado.</returns>
        public async Task<TResponse> GetJsonAsync<TResponse>(
            string relativePath,
            string resourceName,
            CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(relativePath, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new ExternalServiceException(
                    $"SharpAPI returned {(int)response.StatusCode} ({response.ReasonPhrase}) while fetching {resourceName}. {errorBody}");
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var payload = await JsonSerializer.DeserializeAsync<TResponse>(
                responseStream,
                SerializerOptions,
                cancellationToken);

            if (payload is null)
            {
                throw new ExternalServiceException($"SharpAPI returned an empty {resourceName} payload.");
            }

            return payload;
        }
    }
}
