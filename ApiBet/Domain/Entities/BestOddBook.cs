namespace Domain.Entities
{
    /// <summary>
    /// Cuota de una casa concreta dentro de una fila de mejores cuotas.
    /// </summary>
    public class BestOddBook
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected BestOddBook()
        {
        }

        /// <summary>
        /// Crea la cuota de una casa para una selección.
        /// </summary>
        /// <param name="book">Identificador de la casa de apuestas.</param>
        /// <param name="sportsbook">Alias del identificador de la casa.</param>
        /// <param name="american">Cuota americana.</param>
        /// <param name="decimalOdds">Cuota decimal.</param>
        /// <param name="edge">Margen de pago frente al consenso, en porcentaje.</param>
        public BestOddBook(
            string book,
            string sportsbook,
            decimal? american,
            decimal? decimalOdds,
            decimal? edge)
        {
            Id = Guid.NewGuid().ToString("N");
            Book = book ?? string.Empty;
            Sportsbook = string.IsNullOrWhiteSpace(sportsbook) ? Book : sportsbook;
            American = american;
            DecimalOdds = decimalOdds;
            Edge = edge;
        }

        /// <summary>
        /// Identificador único de la fila de all_books.
        /// </summary>
        public string Id { get; protected set; } = string.Empty;

        /// <summary>
        /// Identificador de la fila de mejores cuotas.
        /// </summary>
        public string BestOddId { get; protected set; } = string.Empty;

        /// <summary>
        /// Identificador de la casa de apuestas.
        /// </summary>
        public string Book { get; protected set; } = string.Empty;

        /// <summary>
        /// Alias del identificador de la casa (sportsbook).
        /// </summary>
        public string Sportsbook { get; protected set; } = string.Empty;

        /// <summary>
        /// Cuota americana de la casa.
        /// </summary>
        public decimal? American { get; protected set; }

        /// <summary>
        /// Cuota decimal de la casa.
        /// </summary>
        public decimal? DecimalOdds { get; protected set; }

        /// <summary>
        /// Ventaja porcentual frente al consenso.
        /// </summary>
        public decimal? Edge { get; protected set; }

        /// <summary>
        /// Asocia esta cuota con la fila padre tras calcular el identificador.
        /// </summary>
        /// <param name="bestOddId">Identificador de la fila padre.</param>
        public void AssignBestOdd(string bestOddId)
        {
            BestOddId = bestOddId;
        }
    }
}
