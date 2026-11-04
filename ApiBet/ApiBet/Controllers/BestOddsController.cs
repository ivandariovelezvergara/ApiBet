using Aplication.DTOs;
using Aplication.UseCases.GetBestOdds;
using Aplication.UseCases.SyncBestOdds;
using Aplication.UseCases.SyncOddsDelta;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiBet.Controllers
{
    /// <summary>
    /// Expone las mejores cuotas sincronizadas desde SharpAPI y la carga incremental GET /odds/delta.
    /// </summary>
    [ApiController]
    [Route("api/odds/best")]
    [Tags("Best Odds")]
    [Produces("application/json")]
    public sealed class BestOddsController : ControllerBase
    {
        private readonly ISyncOddsDeltaUseCase _syncOddsDeltaUseCase;
        private readonly ISyncBestOddsUseCase _syncBestOddsUseCase;
        private readonly IGetBestOddsUseCase _getBestOddsUseCase;

        /// <summary>
        /// Inicializa el controlador con los casos de uso de delta y de mejores cuotas.
        /// </summary>
        public BestOddsController(
            ISyncOddsDeltaUseCase syncOddsDeltaUseCase,
            ISyncBestOddsUseCase syncBestOddsUseCase,
            IGetBestOddsUseCase getBestOddsUseCase)
        {
            _syncOddsDeltaUseCase = syncOddsDeltaUseCase;
            _syncBestOddsUseCase = syncBestOddsUseCase;
            _getBestOddsUseCase = getBestOddsUseCase;
        }

        /// <summary>
        /// Vacía MarketOddsDelta y recarga GET /odds/delta con los filtros de OddsDelta (appsettings.json).
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Resumen de la sincronización incremental.</returns>
        [HttpPost("~/api/odds/delta/sync")]
        [ProducesResponseType(typeof(OddsDeltaSyncResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<OddsDeltaSyncResultDto>> SyncDelta(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _syncOddsDeltaUseCase.ExecuteAsync(cancellationToken);
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

        /// <summary>
        /// Ejecuta ahora el ciclo de paginación con OddsSync (appsettings.json) y guarda el resultado en SQL Server.
        /// </summary>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(BestOddsSyncResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<BestOddsSyncResultDto>> Sync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _syncBestOddsUseCase.ExecuteAsync(cancellationToken);
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

        /// <summary>
        /// Calcula value bets con dbo.spr_BestOdds_ValueBets y ValueModel (appsettings.json).
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        [HttpGet]
        [ProducesResponseType(typeof(ValueBetsCatalogDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ValueBetsCatalogDto>> Get(
            CancellationToken cancellationToken = default)
        {
            var result = await _getBestOddsUseCase.ExecuteAsync(cancellationToken);
            return Ok(result);
        }
    }
}
