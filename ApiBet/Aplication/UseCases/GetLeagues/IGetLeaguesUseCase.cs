using Aplication.DTOs;

namespace Aplication.UseCases.GetLeagues
{
    /// <summary>
    /// Caso de uso que consulta las ligas almacenadas en SQL Server.
    /// </summary>
    public interface IGetLeaguesUseCase
    {
        /// <summary>
        /// Obtiene el catálogo persistido localmente.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo local; vacío si todavía no se sincronizó.</returns>
        Task<LeaguesCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
