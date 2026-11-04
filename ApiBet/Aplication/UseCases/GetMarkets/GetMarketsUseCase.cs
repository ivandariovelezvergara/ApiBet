using Aplication.DTOs;
using Domain.Interfaces;

namespace Aplication.UseCases.GetMarkets
{
    /// <summary>
    /// Recupera el catálogo de mercados desde la base SQL Server local.
    /// </summary>
    public sealed class GetMarketsUseCase : IGetMarketsUseCase
    {
        private readonly IMarketRepository _marketRepository;

        /// <summary>
        /// Inicializa el caso de uso con el repositorio de mercados.
        /// </summary>
        /// <param name="marketRepository">Repositorio local de mercados.</param>
        public GetMarketsUseCase(IMarketRepository marketRepository)
        {
            _marketRepository = marketRepository;
        }

        /// <inheritdoc />
        public async Task<MarketsCatalogDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            var catalog = await _marketRepository.GetCatalogAsync(cancellationToken);
            return MarketsCatalogDto.FromDomain(catalog);
        }
    }
}
