namespace Domain.Entities
{
    /// <summary>
    /// Instantánea del catálogo de ligas, ya sea remota o persistida localmente.
    /// </summary>
    public sealed class LeagueCatalog
    {
        /// <summary>
        /// Crea el catálogo con sus ligas y marcas de tiempo.
        /// </summary>
        /// <param name="leagues">Ligas incluidas en la instantánea.</param>
        /// <param name="sourceUpdatedAt">Momento en que SharpAPI actualizó el catálogo.</param>
        /// <param name="lastSyncedAtUtc">Momento local en que se guardó en SQL Server, si aplica.</param>
        public LeagueCatalog(
            IReadOnlyList<League> leagues,
            DateTime? sourceUpdatedAt,
            DateTime? lastSyncedAtUtc = null)
        {
            ArgumentNullException.ThrowIfNull(leagues);

            Leagues = leagues;
            SourceUpdatedAt = sourceUpdatedAt;
            LastSyncedAtUtc = lastSyncedAtUtc;
        }

        /// <summary>
        /// Ligas incluidas en el catálogo.
        /// </summary>
        public IReadOnlyList<League> Leagues { get; }

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
