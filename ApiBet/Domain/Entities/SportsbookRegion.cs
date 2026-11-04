namespace Domain.Entities
{
    /// <summary>
    /// Región asociada a una casa de apuestas.
    /// </summary>
    public class SportsbookRegion
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected SportsbookRegion()
        {
        }

        /// <summary>
        /// Crea la relación entre una casa de apuestas y una región.
        /// </summary>
        /// <param name="sportsbookId">Identificador de la casa padre.</param>
        /// <param name="code">Código de región (por ejemplo, US).</param>
        public SportsbookRegion(string sportsbookId, string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sportsbookId);
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            SportsbookId = sportsbookId;
            Code = code;
        }

        /// <summary>
        /// Identificador de la casa de apuestas.
        /// </summary>
        public string SportsbookId { get; protected set; } = string.Empty;

        /// <summary>
        /// Código de región informado por SharpAPI.
        /// </summary>
        public string Code { get; protected set; } = string.Empty;
    }
}
