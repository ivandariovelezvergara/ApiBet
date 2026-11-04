using Aplication.DTOs;
using Aplication.UseCases.GetLeagues;
using Aplication.UseCases.SyncLeagues;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiBet.Controllers
{
    /// <summary>
    /// Expone el catálogo de ligas sincronizado desde SharpAPI.
    /// </summary>
    [ApiController]
    [Route("api/leagues")]
    [Tags("Leagues")]
    [Produces("application/json")]
    public sealed class LeaguesController : ControllerBase
    {
        private readonly ISyncLeaguesUseCase _syncLeaguesUseCase;
        private readonly IGetLeaguesUseCase _getLeaguesUseCase;

        /// <summary>
        /// Inicializa el controlador con los casos de uso de ligas.
        /// </summary>
        /// <param name="syncLeaguesUseCase">Caso de uso de sincronización remota.</param>
        /// <param name="getLeaguesUseCase">Caso de uso de consulta local.</param>
        public LeaguesController(
            ISyncLeaguesUseCase syncLeaguesUseCase,
            IGetLeaguesUseCase getLeaguesUseCase)
        {
            _syncLeaguesUseCase = syncLeaguesUseCase;
            _getLeaguesUseCase = getLeaguesUseCase;
        }

        /// <summary>
        /// Descarga el catálogo de ligas desde SharpAPI y lo guarda en SQL Server.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo persistido tras la sincronización.</returns>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(LeaguesCatalogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<LeaguesCatalogDto>> Sync(CancellationToken cancellationToken)
        {
            try
            {
                var catalog = await _syncLeaguesUseCase.ExecuteAsync(cancellationToken);
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
        /// Obtiene las ligas almacenadas localmente en SQL Server.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación de la operación.</param>
        /// <returns>Catálogo local; vacío si todavía no se sincronizó.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(LeaguesCatalogDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<LeaguesCatalogDto>> Get(CancellationToken cancellationToken)
        {
            var catalog = await _getLeaguesUseCase.ExecuteAsync(cancellationToken);
            return Ok(catalog);
        }
    }
}
