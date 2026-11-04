using Aplication.DTOs;
using Aplication.UseCases.GetArbitrage;
using Aplication.UseCases.SyncArbitrage;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiBet.Controllers
{
    /// <summary>
    /// Expone las oportunidades de arbitraje sincronizadas desde SharpAPI.
    /// </summary>
    [ApiController]
    [Route("api/arbitrage")]
    [Tags("Arbitrage")]
    [Produces("application/json")]
    public sealed class ArbitrageController : ControllerBase
    {
        private readonly ISyncArbitrageUseCase _syncArbitrageUseCase;
        private readonly IGetArbitrageUseCase _getArbitrageUseCase;

        /// <summary>
        /// Inicializa el controlador con los casos de uso de arbitraje.
        /// </summary>
        public ArbitrageController(
            ISyncArbitrageUseCase syncArbitrageUseCase,
            IGetArbitrageUseCase getArbitrageUseCase)
        {
            _syncArbitrageUseCase = syncArbitrageUseCase;
            _getArbitrageUseCase = getArbitrageUseCase;
        }

        /// <summary>
        /// Descarga GET /opportunities/arbitrage con los filtros de ArbitrageFeed (appsettings.json) y lo guarda en SQL Server.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Resumen de la sincronización.</returns>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(ArbitrageSyncResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<ArbitrageSyncResultDto>> Sync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _syncArbitrageUseCase.ExecuteAsync(cancellationToken);
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
        /// Devuelve las boletas de 2 vías con los filtros de ArbitrageFeed y ValueModel.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Boletas con montos en múltiplos de 100, incluida CreatedAtUtc.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ArbitrageCatalogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ArbitrageCatalogDto>> Get(CancellationToken cancellationToken)
        {
            try
            {
                var catalog = await _getArbitrageUseCase.ExecuteAsync(cancellationToken);
                return Ok(catalog);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }
    }
}
