using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Contrato de persistencia de las mejores cuotas en SQL Server.
    /// </summary>
    public interface IBestOddsRepository
    {
        /// <summary>
        /// Vacía las tablas del flujo de mejores cuotas antes de recargar el catálogo.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        Task ClearAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Inserta o actualiza una página de cuotas en la misma transacción.
        /// </summary>
        /// <param name="odds">Filas de la iteración actual.</param>
        /// <param name="sourceUpdatedAt">Marca de tiempo del origen.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Cantidad de filas padre y de casas persistidas en la página.</returns>
        Task<(int OddCount, int BookCount)> SavePageAsync(
            IReadOnlyList<BestOdd> odds,
            DateTime? sourceUpdatedAt,
            CancellationToken cancellationToken);

        /// <summary>
        /// Recupera todas las mejores cuotas mediante dbo.spr_BestOdds.
        /// </summary>
        /// <param name="sport">Slug de deporte; nulo para devolver todas.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Listado plano, una fila por cada casa de all_books.</returns>
        Task<IReadOnlyList<BestOddRow>> GetAsync(string? sport, CancellationToken cancellationToken);

        /// <summary>
        /// Ejecuta dbo.spr_BestOdds_ValueBets y devuelve picks más resumen.
        /// </summary>
        /// <param name="query">Parámetros financieros y de ventana.</param>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Picks de la cartera y resumen por deporte/liga.</returns>
        Task<ValueBetCatalog> GetValueBetsAsync(ValueBetQuery query, CancellationToken cancellationToken);
    }
}
