using Aplication.Configuration;
using Aplication.DTOs;
using Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Aplication.UseCases.GetBestOdds
{
    /// <summary>
    /// Obtiene value bets mediante dbo.spr_BestOdds_ValueBets.
    /// </summary>
    public sealed class GetBestOddsUseCase : IGetBestOddsUseCase
    {
        private readonly IBestOddsRepository _bestOddsRepository;
        private readonly IOptionsSnapshot<ValueModelOptions> _valueModel;

        /// <summary>
        /// Inicializa el caso de uso con el repositorio y ValueModel.
        /// </summary>
        public GetBestOddsUseCase(
            IBestOddsRepository bestOddsRepository,
            IOptionsSnapshot<ValueModelOptions> valueModel)
        {
            _bestOddsRepository = bestOddsRepository;
            _valueModel = valueModel;
        }

        /// <inheritdoc />
        public async Task<ValueBetsCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var catalog = await _bestOddsRepository.GetValueBetsAsync(
                _valueModel.Value.ToValueBetQuery(),
                cancellationToken);

            return ValueBetsCatalogDto.FromDomain(catalog);
        }
    }
}
