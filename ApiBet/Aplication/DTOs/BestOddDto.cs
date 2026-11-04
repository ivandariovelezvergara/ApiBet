using Domain.Entities;

namespace Aplication.DTOs
{
    /// <summary>
    /// Fila de mejores cuotas alineada con el JSON de dbo.spr_BestOdds.
    /// </summary>
    public sealed class BestOddDto
    {
        /// <summary>
        /// Identificador del evento.
        /// </summary>
        public required string EventId { get; init; }

        /// <summary>
        /// Nombre del evento.
        /// </summary>
        public required string EventName { get; init; }

        /// <summary>
        /// Nombre del deporte.
        /// </summary>
        public required string SportName { get; init; }

        /// <summary>
        /// Nombre visible de la liga.
        /// </summary>
        public required string LeagueName { get; init; }

        /// <summary>
        /// Nombre visible del mercado.
        /// </summary>
        public required string MarketName { get; init; }

        /// <summary>
        /// Selección.
        /// </summary>
        public required string Selection { get; init; }

        /// <summary>
        /// Línea; nula si no aplica.
        /// </summary>
        public decimal? Line { get; init; }

        /// <summary>
        /// Indica si es línea principal.
        /// </summary>
        public bool IsMainLine { get; init; }

        /// <summary>
        /// Mejor cuota americana.
        /// </summary>
        public decimal? BestAmerican { get; init; }

        /// <summary>
        /// Mejor cuota decimal.
        /// </summary>
        public decimal? BestDecimal { get; init; }

        /// <summary>
        /// Probabilidad implícita de la mejor cuota.
        /// </summary>
        public decimal? BestProbability { get; init; }

        /// <summary>
        /// Cuota americana de consenso.
        /// </summary>
        public decimal? ConsensusAmerican { get; init; }

        /// <summary>
        /// Cuota decimal de consenso.
        /// </summary>
        public decimal? ConsensusDecimal { get; init; }

        /// <summary>
        /// Probabilidad implícita de consenso.
        /// </summary>
        public decimal? ConsensusProbability { get; init; }

        /// <summary>
        /// Hold del mercado.
        /// </summary>
        public decimal? MarketHold { get; init; }

        /// <summary>
        /// Nombre visible de la casa con la mejor cuota.
        /// </summary>
        public required string BestBookDisplayName { get; init; }

        /// <summary>
        /// Identificador de la casa en all_books.
        /// </summary>
        public required string Book { get; init; }

        /// <summary>
        /// Nombre visible de la casa en all_books.
        /// </summary>
        public required string SportsbookDisplayName { get; init; }

        /// <summary>
        /// Cuota americana de la casa.
        /// </summary>
        public decimal? BookAmerican { get; init; }

        /// <summary>
        /// Cuota decimal de la casa.
        /// </summary>
        public decimal? BookDecimal { get; init; }

        /// <summary>
        /// Margen de la casa frente al consenso.
        /// </summary>
        public decimal? Edge { get; init; }

        /// <summary>
        /// Proyecta la fila del procedimiento hacia el DTO de la API.
        /// </summary>
        public static BestOddDto FromRow(BestOddRow row)
        {
            return new BestOddDto
            {
                EventId = row.EventId,
                EventName = row.EventName,
                SportName = row.SportName,
                LeagueName = row.LeagueName,
                MarketName = row.MarketName,
                Selection = row.Selection,
                Line = row.Line,
                IsMainLine = row.IsMainLine,
                BestAmerican = row.BestAmerican,
                BestDecimal = row.BestDecimal,
                BestProbability = row.BestProbability,
                ConsensusAmerican = row.ConsensusAmerican,
                ConsensusDecimal = row.ConsensusDecimal,
                ConsensusProbability = row.ConsensusProbability,
                MarketHold = row.MarketHold,
                BestBookDisplayName = row.BestBookDisplayName,
                Book = row.Book,
                SportsbookDisplayName = row.SportsbookDisplayName,
                BookAmerican = row.BookAmerican,
                BookDecimal = row.BookDecimal,
                Edge = row.Edge
            };
        }
    }
}
