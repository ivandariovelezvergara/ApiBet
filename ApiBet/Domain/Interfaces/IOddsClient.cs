using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Abstracción para consultar GET /odds y GET /odds/delta en SharpAPI.
    /// </summary>
    public interface IOddsClient
    {
        /// <summary>
        /// Obtiene una página de cuotas. Si se envía cursor, tiene prioridad sobre offset.
        /// </summary>
        /// <param name="limit">Tamaño de página (máximo 200).</param>
        /// <param name="sports">Slugs de deporte separados por coma; nulo o vacío para omitir el filtro.</param>
        /// <param name="market">Filtro market de SharpAPI; nulo o vacío para omitirlo.</param>
        /// <param name="isLive">Filtro is_live de SharpAPI; nulo para omitirlo.</param>
        /// <param name="isMainLine">Filtro is_main_line de SharpAPI; nulo para omitirlo.</param>
        /// <param name="cursor">Cursor opaco de una respuesta anterior; tiene prioridad sobre offset.</param>
        /// <param name="offset">Desplazamiento de paginación; se omite cuando hay cursor.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Página de cuotas de mercado.</returns>
        Task<MarketOddsPage> GetOddsAsync(
            int limit,
            string? sports,
            string? market,
            bool? isLive,
            bool? isMainLine,
            string? cursor,
            int? offset,
            CancellationToken cancellationToken);

        /// <summary>
        /// Obtiene una página incremental de cuotas (altas, actualizaciones y retiros).
        /// </summary>
        /// <param name="since">Marca ISO 8601 UTC desde la que se piden cambios.</param>
        /// <param name="limit">Tamaño de página (máximo 500).</param>
        /// <param name="sports">Slugs de deporte separados por coma; nulo o vacío para omitir el filtro.</param>
        /// <param name="market">Filtro market de SharpAPI; nulo o vacío para omitirlo.</param>
        /// <param name="isLive">Filtro is_live de SharpAPI; nulo para omitirlo.</param>
        /// <param name="isMainLine">Filtro is_main_line de SharpAPI; nulo para omitirlo.</param>
        /// <param name="cursor">Cursor opaco de una respuesta anterior; tiene prioridad sobre offset.</param>
        /// <param name="offset">Desplazamiento de paginación; se omite cuando hay cursor.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Página incremental con cuotas activas y retiradas.</returns>
        Task<MarketOddsDeltaPage> GetOddsDeltaAsync(
            string since,
            int limit,
            string? sports,
            string? market,
            bool? isLive,
            bool? isMainLine,
            string? cursor,
            int? offset,
            CancellationToken cancellationToken);
    }
}
