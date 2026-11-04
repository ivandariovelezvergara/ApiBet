using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Representación de un mercado para la capa de aplicación.
    /// </summary>
    public sealed class MarketDto
    {
        /// <summary>
        /// Slug del mercado, valor a usar en filtros de cuotas.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Etiqueta visible; no se usa como filtro.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Identificador numérico estable; nulo si SharpAPI no lo publica.
        /// </summary>
        public int? NumericalId { get; init; }

        /// <summary>
        /// Cantidad de eventos que ofrecen este mercado.
        /// </summary>
        public int EventCount { get; init; }

        /// <summary>
        /// Identificadores de deportes que actualmente tienen este mercado.
        /// </summary>
        public required IReadOnlyList<string> Sports { get; init; }

        /// <summary>
        /// Proyecta la entidad de dominio hacia el DTO de aplicación.
        /// </summary>
        /// <param name="market">Entidad persistida o recibida del proveedor.</param>
        /// <returns>DTO listo para exponer en la API.</returns>
        public static MarketDto FromDomain(Market market)
        {
            return new MarketDto
            {
                Id = market.Id,
                Name = market.Name,
                NumericalId = market.NumericalId,
                EventCount = market.EventCount,
                Sports = market.Sports
                    .Select(sport => sport.SportId)
                    .OrderBy(sportId => sportId, StringComparer.OrdinalIgnoreCase)
                    .ToArray()
            };
        }
    }
}
