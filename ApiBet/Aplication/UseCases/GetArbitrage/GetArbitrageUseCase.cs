using Aplication.Configuration;
using Aplication.DTOs;
using Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Aplication.UseCases.GetArbitrage
{
    /// <summary>
    /// Recupera las boletas de arbitraje mediante dbo.spr_ArbitrageOpportunities.
    /// </summary>
    public sealed class GetArbitrageUseCase : IGetArbitrageUseCase
    {
        private readonly IArbitrageOpportunityRepository _arbitrageRepository;
        private readonly IOptionsSnapshot<ArbitrageFeedOptions> _feed;
        private readonly ValueModelOptions _valueModel;

        /// <summary>
        /// Inicializa el caso de uso con el repositorio, ArbitrageFeed y la banca.
        /// </summary>
        public GetArbitrageUseCase(
            IArbitrageOpportunityRepository arbitrageRepository,
            IOptionsSnapshot<ArbitrageFeedOptions> feed,
            IOptions<ValueModelOptions> valueModel)
        {
            _arbitrageRepository = arbitrageRepository;
            _feed = feed;
            _valueModel = valueModel.Value;
        }

        /// <inheritdoc />
        public async Task<ArbitrageCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var options = _feed.Value;
            var bankroll = _valueModel.BankrollCop;
            if (bankroll < 200)
            {
                throw new InvalidOperationException("Bankroll must be at least 200 COP.");
            }

            var rows = await _arbitrageRepository.GetBetsAsync(
                bankroll,
                options.GetSportFilter(),
                options.MinProfit ?? 0,
                options.IsLive,
                cancellationToken);

            return ArbitrageCatalogDto.FromDomain(rows);
        }
    }
}
