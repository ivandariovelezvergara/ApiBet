using Aplication.DTOs;
using Aplication.UseCases.GetMarkets;
using Aplication.UseCases.SyncMarkets;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiBet.Controllers
{
    /// <summary>
    /// Expone el catálogo de mercados sincronizado desde SharpAPI.
    /// </summary>
    [ApiController]
    [Route("api/markets")]
    [Tags("Markets")]
    [Produces("application/json")]
    public sealed class MarketsController : ControllerBase
    {
        private readonly ISyncMarketsUseCase _syncMarketsUseCase;
        private readonly IGetMarketsUseCase _getMarketsUseCase;

        /// <summary>
        /// Inicializa el controlador con los casos de uso de mercados.
        /// </summary>
        /// <param name="syncMarketsUseCase">Caso de uso de sincronización remota.</param>
        /// <param name="getMarketsUseCase">Caso de uso de consulta local.</param>
        public MarketsController(
            ISyncMarketsUseCase syncMarketsUseCase,
            IGetMarketsUseCase getMarketsUseCase)
        {
            _syncMarketsUseCase = syncMarketsUseCase;
            _getMarketsUseCase = getMarketsUseCase;
        }

        /// <summary>
        /// Descarga el catálogo de mercados desde SharpAPI y lo guarda en SQL Server.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido tras la sincronización.</returns>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(MarketsCatalogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<MarketsCatalogDto>> Sync(CancellationToken cancellationToken)
        {
            try
            {
                var catalog = await _syncMarketsUseCase.ExecuteAsync(cancellationToken);
                return Ok(catalog);
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
        /// Obtiene los mercados almacenados localmente en SQL Server.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo local; vacío si todavía no se sincronizó.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(MarketsCatalogDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<MarketsCatalogDto>> Get(CancellationToken cancellationToken)
        {
            var catalog = await _getMarketsUseCase.ExecuteAsync(cancellationToken);
            return Ok(catalog);
        }
    }
}
