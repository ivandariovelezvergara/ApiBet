using Aplication.DTOs;
using Aplication.UseCases.GetSportsbooks;
using Aplication.UseCases.SyncSportsbooks;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiBet.Controllers
{
    /// <summary>
    /// Expone el catálogo de casas de apuestas sincronizado desde SharpAPI.
    /// </summary>
    [ApiController]
    [Route("api/sportsbooks")]
    [Tags("Sportsbooks")]
    [Produces("application/json")]
    public sealed class SportsbooksController : ControllerBase
    {
        private readonly ISyncSportsbooksUseCase _syncSportsbooksUseCase;
        private readonly IGetSportsbooksUseCase _getSportsbooksUseCase;

        /// <summary>
        /// Inicializa el controlador con los casos de uso de casas de apuestas.
        /// </summary>
        /// <param name="syncSportsbooksUseCase">Caso de uso de sincronización remota.</param>
        /// <param name="getSportsbooksUseCase">Caso de uso de consulta local.</param>
        public SportsbooksController(
            ISyncSportsbooksUseCase syncSportsbooksUseCase,
            IGetSportsbooksUseCase getSportsbooksUseCase)
        {
            _syncSportsbooksUseCase = syncSportsbooksUseCase;
            _getSportsbooksUseCase = getSportsbooksUseCase;
        }

        /// <summary>
        /// Descarga el catálogo de casas desde SharpAPI y lo guarda en SQL Server.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido tras la sincronización.</returns>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(SportsbooksCatalogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<SportsbooksCatalogDto>> Sync(CancellationToken cancellationToken)
        {
            try
            {
                var catalog = await _syncSportsbooksUseCase.ExecuteAsync(cancellationToken);
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
        /// Obtiene las casas de apuestas almacenadas localmente en SQL Server.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo local; vacío si todavía no se sincronizó.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(SportsbooksCatalogDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SportsbooksCatalogDto>> Get(CancellationToken cancellationToken)
        {
            var catalog = await _getSportsbooksUseCase.ExecuteAsync(cancellationToken);
            return Ok(catalog);
        }
    }
}
