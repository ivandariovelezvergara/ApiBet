namespace Domain.Entities
{
    /// <summary>
    /// Instantánea del catálogo de deportes, ya sea remota o persistida localmente.
    /// </summary>
    public sealed class SportCatalog
    {
        /// <summary>
        /// Crea el catálogo con sus deportes y marcas de tiempo.
        /// </summary>
        /// <param name="sports">Deportes incluidos en la instantánea.</param>
        /// <param name="sourceUpdatedAt">Momento en que SharpAPI actualizó el catálogo.</param>
        /// <param name="lastSyncedAtUtc">Momento local en que se guardó en SQL Server, si aplica.</param>
        public SportCatalog(
            IReadOnlyList<Sport> sports,
            DateTime? sourceUpdatedAt,
            DateTime? lastSyncedAtUtc = null)
        {
            ArgumentNullException.ThrowIfNull(sports);

            Sports = sports;
            SourceUpdatedAt = sourceUpdatedAt;
            LastSyncedAtUtc = lastSyncedAtUtc;
        }

        /// <summary>
        /// Deportes incluidos en el catálogo.
        /// </summary>
        public IReadOnlyList<Sport> Sports { get; }

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
