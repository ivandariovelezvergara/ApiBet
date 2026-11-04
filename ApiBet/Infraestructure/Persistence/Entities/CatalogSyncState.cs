namespace Infraestructure.Persistence.Entities
{
    /// <summary>
    /// Estado de sincronización del catálogo persistido en SQL Server.
    /// </summary>
    public sealed class CatalogSyncState
    {
        /// <summary>
        /// Identificador único de la fila de metadatos (siempre 1).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha de actualización informada por SharpAPI.
        /// </summary>
        public DateTime? SourceUpdatedAt { get; set; }

        /// <summary>
        /// Fecha UTC en que se guardó el catálogo localmente.
        /// </summary>
        public DateTime LastSyncedAtUtc { get; set; }
    }
}
