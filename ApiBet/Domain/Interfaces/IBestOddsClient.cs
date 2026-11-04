using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Abstracción para consultar las mejores cuotas en SharpAPI.
    /// </summary>
    public interface IBestOddsClient
    {
        /// <summary>
        /// Obtiene una página de mejores cuotas según deporte, límite, offset y filtros de mercado.
        /// </summary>
        /// <param name="limit">Tamaño de página (máximo 200).</param>
        /// <param name="offset">Desplazamiento de paginación.</param>
        /// <param name="sports">Slugs de deporte separados por coma.</param>
        /// <param name="market">Filtro market de SharpAPI; nulo o vacío para omitirlo.</param>
        /// <param name="isMainLine">Filtro is_main_line de SharpAPI; nulo para omitirlo.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Página de mejores cuotas.</returns>
        Task<BestOddsPage> GetBestOddsAsync(
            int limit,
            int offset,
            string sports,
            string? market,
            bool? isMainLine,
            CancellationToken cancellationToken);
    }
}
