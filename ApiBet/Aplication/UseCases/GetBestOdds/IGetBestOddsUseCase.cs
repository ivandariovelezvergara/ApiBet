using Aplication.DTOs;

namespace Aplication.UseCases.GetBestOdds
{
    /// <summary>
    /// Caso de uso que calcula value bets con dbo.spr_BestOdds_ValueBets.
    /// </summary>
    public interface IGetBestOddsUseCase
    {
        /// <summary>
        /// Ejecuta el SP con ValueModel de appsettings.json.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Picks y resumen ejecutivo.</returns>
        Task<ValueBetsCatalogDto> ExecuteAsync(CancellationToken cancellationToken);
    }
}
