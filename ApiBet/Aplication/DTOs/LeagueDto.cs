using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Representación de una liga para la capa de aplicación.
    /// </summary>
    public sealed class LeagueDto
    {
        /// <summary>
        /// Identificador de la liga.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Nombre visible de la liga.
        /// </summary>
        public required string DisplayName { get; init; }

        /// <summary>
        /// Identificador numérico estable; nulo si SharpAPI no lo publica.
        /// </summary>
        public int? NumericalId { get; init; }

        /// <summary>
        /// Identificador del deporte al que pertenece.
        /// </summary>
        public required string Sport { get; init; }

        /// <summary>
        /// Cantidad total de eventos con cuotas.
        /// </summary>
        public int EventCount { get; init; }

        /// <summary>
        /// Cantidad de eventos en vivo.
        /// </summary>
        public int LiveCount { get; init; }

        /// <summary>
        /// Proyecta la entidad de dominio hacia el DTO de aplicación.
        /// </summary>
        /// <param name="league">Entidad de liga persistida o recibida del proveedor.</param>
        /// <returns>DTO listo para exponer en la API.</returns>
        public static LeagueDto FromDomain(League league)
        {
            return new LeagueDto
            {
                Id = league.Id,
                DisplayName = league.DisplayName,
                NumericalId = league.NumericalId,
                Sport = league.SportId,
                EventCount = league.EventCount,
                LiveCount = league.LiveCount
            };
        }
    }
}
