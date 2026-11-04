using Aplication.DTOs;
using Aplication.UseCases.SyncOdds;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiBet.Controllers
{
    /// <summary>
    /// Expone la sincronización de cuotas GET /odds de SharpAPI.
    /// </summary>
    [ApiController]
    [Route("api/odds")]
    [Tags("Odds")]
    [Produces("application/json")]
    public sealed class OddsController : ControllerBase
    {
        private readonly ISyncOddsUseCase _syncOddsUseCase;

        /// <summary>
        /// Inicializa el controlador con el caso de uso de sincronización de MarketOdds.
        /// </summary>
        public OddsController(ISyncOddsUseCase syncOddsUseCase)
        {
            _syncOddsUseCase = syncOddsUseCase;
        }

        /// <summary>
        /// Vacía MarketOdds y recarga GET /odds con los filtros de OddsFeed (appsettings.json).
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Resumen de la sincronización.</returns>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(OddsSyncResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<OddsSyncResultDto>> Sync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _syncOddsUseCase.ExecuteAsync(cancellationToken);
                return Ok(result);
            }
            catch (ExternalServiceException exception)
            {
                return Problem(
                    detail: exception.Message,
                    statusCode: StatusCodes.Status502BadGateway,
                    title: "SharpAPI request failed");
            }
        }
    }
}
