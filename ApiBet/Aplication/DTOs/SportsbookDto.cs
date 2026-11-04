using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Representación de una casa de apuestas para la capa de aplicación.
    /// </summary>
    public sealed class SportsbookDto
    {
        /// <summary>
        /// Identificador único de la casa.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Identificador numérico estable; nulo si SharpAPI no lo publica.
        /// </summary>
        public int? NumericalId { get; init; }

        /// <summary>
        /// Nombre legible de la casa.
        /// </summary>
        public required string DisplayName { get; init; }

        /// <summary>
        /// Nombre corto para mostrar.
        /// </summary>
        public required string ShortName { get; init; }

        /// <summary>
        /// Categoría del libro.
        /// </summary>
        public required string Category { get; init; }

        /// <summary>
        /// Indica si ofrece cuotas en vivo.
        /// </summary>
        public bool HasLiveOdds { get; init; }

        /// <summary>
        /// Indica si ofrece mercados de jugador.
        /// </summary>
        public bool HasPlayerProps { get; init; }

        /// <summary>
        /// Indica si publica límites; nulo si no se ha observado.
        /// </summary>
        public bool? HasLimits { get; init; }

        /// <summary>
        /// Regiones disponibles.
        /// </summary>
        public required IReadOnlyList<string> Regions { get; init; }

        /// <summary>
        /// Indica si se considera un mercado sharp.
        /// </summary>
        public bool IsSharp { get; init; }

        /// <summary>
        /// Estado operativo (active o inactive).
        /// </summary>
        public required string Status { get; init; }

        /// <summary>
        /// Eventos disponibles actualmente.
        /// </summary>
        public int EventCount { get; init; }

        /// <summary>
        /// Última actualización de datos del libro.
        /// </summary>
        public DateTime? LastUpdate { get; init; }

        /// <summary>
        /// Nivel mínimo requerido.
        /// </summary>
        public required string RequiresTier { get; init; }

        /// <summary>
        /// Indica si el libro está anunciado y aún no disponible.
        /// </summary>
        public bool ComingSoon { get; init; }

        /// <summary>
        /// Proyecta la entidad de dominio hacia el DTO de aplicación.
        /// </summary>
        /// <param name="sportsbook">Entidad persistida o recibida del proveedor.</param>
        /// <returns>DTO listo para exponer en la API.</returns>
        public static SportsbookDto FromDomain(Sportsbook sportsbook)
        {
            return new SportsbookDto
            {
                Id = sportsbook.Id,
                NumericalId = sportsbook.NumericalId,
                DisplayName = sportsbook.DisplayName,
                ShortName = sportsbook.ShortName,
                Category = sportsbook.Category,
                HasLiveOdds = sportsbook.HasLiveOdds,
                HasPlayerProps = sportsbook.HasPlayerProps,
                HasLimits = sportsbook.HasLimits,
                Regions = sportsbook.Regions
                    .Select(region => region.Code)
                    .OrderBy(code => code, StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                IsSharp = sportsbook.IsSharp,
                Status = sportsbook.Status,
                EventCount = sportsbook.EventCount,
                LastUpdate = sportsbook.LastUpdate,
                RequiresTier = sportsbook.RequiresTier,
                ComingSoon = sportsbook.ComingSoon
            };
        }
    }
}
