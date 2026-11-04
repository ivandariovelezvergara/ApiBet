using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Catálogo de ligas expuesto por los casos de uso.
    /// </summary>
    public sealed class LeaguesCatalogDto
    {
        /// <summary>
        /// Ligas del catálogo.
        /// </summary>
        public required IReadOnlyList<LeagueDto> Data { get; init; }

        /// <summary>
        /// Marca de tiempo de actualización informada por SharpAPI.
        /// </summary>
        public DateTime? UpdatedAt { get; init; }

        /// <summary>
        /// Marca de tiempo UTC de la última persistencia local.
        /// </summary>
        public DateTime? LastSyncedAtUtc { get; init; }

        /// <summary>
        /// Proyecta el catálogo de dominio hacia el DTO de aplicación.
        /// </summary>
        /// <param name="catalog">Catálogo de dominio.</param>
        /// <returns>DTO listo para exponer en la API.</returns>
        public static LeaguesCatalogDto FromDomain(LeagueCatalog catalog)
        {
            return new LeaguesCatalogDto
            {
                Data = catalog.Leagues.Select(LeagueDto.FromDomain).ToArray(),
                UpdatedAt = catalog.SourceUpdatedAt,
                LastSyncedAtUtc = catalog.LastSyncedAtUtc
            };
        }
    }
}
