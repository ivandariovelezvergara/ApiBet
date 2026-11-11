using Aplication.DTOs;
using Aplication.UseCases.CreatePlacedBet;
using Aplication.UseCases.GetPlacedBets;
using Aplication.UseCases.UpdatePlacedBetStatus;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ApiBet.Controllers
{
    /// <summary>
    /// Expone la bitácora de apuestas ejecutadas (PlacedBets), su CLV y su liquidación (Won/Lost/Push).
    /// </summary>
    [ApiController]
    [Route("api/bets")]
    [Tags("Placed Bets")]
    [Produces("application/json")]
    public sealed class PlacedBetsController : ControllerBase
    {
        private readonly ICreatePlacedBetUseCase _createPlacedBetUseCase;
        private readonly IUpdatePlacedBetStatusUseCase _updatePlacedBetStatusUseCase;
        private readonly IGetPlacedBetsUseCase _getPlacedBetsUseCase;

        /// <summary>
        /// Inicializa el controlador con los casos de uso de bitácora de apuestas.
        /// </summary>
        public PlacedBetsController(
            ICreatePlacedBetUseCase createPlacedBetUseCase,
            IUpdatePlacedBetStatusUseCase updatePlacedBetStatusUseCase,
            IGetPlacedBetsUseCase getPlacedBetsUseCase)
        {
            _createPlacedBetUseCase = createPlacedBetUseCase;
            _updatePlacedBetStatusUseCase = updatePlacedBetStatusUseCase;
            _getPlacedBetsUseCase = getPlacedBetsUseCase;
        }

        /// <summary>
        /// Obtiene todas las apuestas registradas junto con su CLV y resumen de ROI.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PlacedBetsCatalogDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PlacedBetsCatalogDto>> Get(CancellationToken cancellationToken)
        {
            var catalog = await _getPlacedBetsUseCase.ExecuteAsync(cancellationToken);
            return Ok(catalog);
        }

        /// <summary>
        /// Registra una apuesta ejecutada en dbo.PlacedBets.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PlacedBet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PlacedBet>> Create(
            [FromBody] CreatePlacedBetRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var created = await _createPlacedBetUseCase.ExecuteAsync(request, cancellationToken);
                return Ok(created);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        /// <summary>
        /// Liquida o cambia el estado de una apuesta registrada (WON, LOST, PUSH, VOID, PENDING).
        /// </summary>
        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(typeof(PlacedBet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlacedBet>> UpdateStatus(
            int id,
            [FromBody] UpdatePlacedBetStatusRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _updatePlacedBetStatusUseCase.ExecuteAsync(id, request.Estado, cancellationToken);
                if (updated is null)
                {
                    return NotFound(new { message = $"No existe la apuesta con Id {id}." });
                }

                return Ok(updated);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }
    }
}
