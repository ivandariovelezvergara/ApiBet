using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Listado de boletas de arbitraje expuesto por GET /api/arbitrage.
    /// </summary>
    public sealed class ArbitrageCatalogDto
    {
        /// <summary>
        /// Boletas de 2 vías, ordenadas por ganancia esperada descendente.
        /// </summary>
        public required IReadOnlyList<ArbitrageBetDto> Data { get; init; }

        /// <summary>
        /// Proyecta las filas del SP hacia el DTO de aplicación.
        /// </summary>
        public static ArbitrageCatalogDto FromDomain(IReadOnlyList<ArbitrageBetRow> rows)
        {
            return new ArbitrageCatalogDto
            {
                Data = rows.Select(ArbitrageBetDto.FromDomain).ToArray()
            };
        }
    }
}
