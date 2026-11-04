namespace Domain.Entities
{
    /// <summary>
    /// Liga asociada a un deporte del catálogo.
    /// </summary>
    public class SportLeague
    {
        /// <summary>
        /// Constructor requerido por el mapeo de persistencia.
        /// </summary>
        protected SportLeague()
        {
        }

        /// <summary>
        /// Crea la relación entre un deporte y el código de una liga.
        /// </summary>
        /// <param name="sportId">Identificador del deporte padre.</param>
        /// <param name="code">Código de la liga (por ejemplo, atp).</param>
        public SportLeague(string sportId, string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sportId);
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            SportId = sportId;
            Code = code;
        }

        /// <summary>
        /// Identificador del deporte al que pertenece la liga.
        /// </summary>
        public string SportId { get; protected set; } = string.Empty;

        /// <summary>
        /// Código de liga informado por SharpAPI.
        /// </summary>
        public string Code { get; protected set; } = string.Empty;
    }
}
