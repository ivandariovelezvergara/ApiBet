using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Representación de un deporte para la capa de aplicación.
    /// </summary>
    public sealed class SportDto
    {
        /// <summary>
        /// Identificador del deporte.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Nombre visible del deporte.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Identificador numérico estable; nulo si SharpAPI no lo publica.
        /// </summary>
        public int? NumericalId { get; init; }

        /// <summary>
        /// Cantidad total de eventos con cuotas.
        /// </summary>
        public int EventCount { get; init; }

        /// <summary>
        /// Cantidad de eventos en vivo.
        /// </summary>
        public int LiveCount { get; init; }

        /// <summary>
        /// Códigos de ligas asociadas.
        /// </summary>
        public required IReadOnlyList<string> Leagues { get; init; }

        /// <summary>
        /// Proyecta la entidad de dominio hacia el DTO de aplicación.
        /// </summary>
        /// <param name="sport">Entidad de deporte persistida o recibida del proveedor.</param>
        /// <returns>DTO listo para exponer en la API.</returns>
        public static SportDto FromDomain(Sport sport)
        {
            return new SportDto
            {
                Id = sport.Id,
                Name = sport.Name,
                NumericalId = sport.NumericalId,
                EventCount = sport.EventCount,
                LiveCount = sport.LiveCount,
                Leagues = sport.Leagues
                    .Select(league => league.Code)
                    .OrderBy(code => code, StringComparer.OrdinalIgnoreCase)
                    .ToArray()
            };
        }
    }
}
