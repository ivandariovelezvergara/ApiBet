using Aplication.DTOs;
using Aplication.UseCases.GetSports;
using Aplication.UseCases.SyncSports;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiBet.Controllers
{
    /// <summary>
    /// Expone el catálogo de deportes sincronizado desde SharpAPI.
    /// </summary>
    [ApiController]
    [Route("api/sports")]
    [Tags("Sports")]
    [Produces("application/json")]
    public sealed class SportsController : ControllerBase
    {
        private readonly ISyncSportsUseCase _syncSportsUseCase;
        private readonly IGetSportsUseCase _getSportsUseCase;

        /// <summary>
        /// Inicializa el controlador con los casos de uso de deportes.
        /// </summary>
        /// <param name="syncSportsUseCase">Caso de uso de sincronización remota.</param>
        /// <param name="getSportsUseCase">Caso de uso de consulta local.</param>
        public SportsController(
            ISyncSportsUseCase syncSportsUseCase,
            IGetSportsUseCase getSportsUseCase)
        {
            _syncSportsUseCase = syncSportsUseCase;
            _getSportsUseCase = getSportsUseCase;
        }

        /// <summary>
        /// Descarga el catálogo desde SharpAPI y lo guarda en SQL Server.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido tras la sincronización.</returns>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(SportsCatalogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<SportsCatalogDto>> Sync(CancellationToken cancellationToken)
        {
            try
            {
                var catalog = await _syncSportsUseCase.ExecuteAsync(cancellationToken);
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
        /// Obtiene los deportes almacenados localmente en SQL Server.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo local; vacío si todavía no se sincronizó.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(SportsCatalogDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SportsCatalogDto>> Get(CancellationToken cancellationToken)
        {
            var catalog = await _getSportsUseCase.ExecuteAsync(cancellationToken);
            return Ok(catalog);
        }
    }
}
