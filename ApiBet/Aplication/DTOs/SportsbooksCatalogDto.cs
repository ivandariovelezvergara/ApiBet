using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Catálogo de casas de apuestas expuesto por los casos de uso.
    /// </summary>
    public sealed class SportsbooksCatalogDto
    {
        /// <summary>
        /// Casas de apuestas del catálogo.
        /// </summary>
        public required IReadOnlyList<SportsbookDto> Data { get; init; }

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
        public static SportsbooksCatalogDto FromDomain(SportsbookCatalog catalog)
        {
            return new SportsbooksCatalogDto
            {
                Data = catalog.Sportsbooks.Select(SportsbookDto.FromDomain).ToArray(),
                UpdatedAt = catalog.SourceUpdatedAt,
                LastSyncedAtUtc = catalog.LastSyncedAtUtc
            };
        }
    }
}
