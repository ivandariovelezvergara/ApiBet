namespace Domain.Entities
{
    /// <summary>
    /// Instantánea del catálogo de casas de apuestas, remota o persistida localmente.
    /// </summary>
    public sealed class SportsbookCatalog
    {
        /// <summary>
        /// Crea el catálogo con sus casas y marcas de tiempo.
        /// </summary>
        /// <param name="sportsbooks">Casas incluidas en la instantánea.</param>
        /// <param name="sourceUpdatedAt">Momento en que SharpAPI actualizó el catálogo.</param>
        /// <param name="lastSyncedAtUtc">Momento local en que se guardó en SQL Server, si aplica.</param>
        public SportsbookCatalog(
            IReadOnlyList<Sportsbook> sportsbooks,
            DateTime? sourceUpdatedAt,
            DateTime? lastSyncedAtUtc = null)
        {
            ArgumentNullException.ThrowIfNull(sportsbooks);

            Sportsbooks = sportsbooks;
            SourceUpdatedAt = sourceUpdatedAt;
            LastSyncedAtUtc = lastSyncedAtUtc;
        }

        /// <summary>
        /// Casas de apuestas incluidas en el catálogo.
        /// </summary>
        public IReadOnlyList<Sportsbook> Sportsbooks { get; }

        /// <summary>
        /// Fecha y hora en que SharpAPI actualizó el origen.
        /// </summary>
        public DateTime? SourceUpdatedAt { get; }

        /// <summary>
        /// Fecha y hora UTC de la última sincronización local.
        /// </summary>
        public DateTime? LastSyncedAtUtc { get; }
    }
}
