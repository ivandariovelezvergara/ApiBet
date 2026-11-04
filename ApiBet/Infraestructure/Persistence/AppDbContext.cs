using Domain.Entities;
using Infraestructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence
{
    /// <summary>
    /// Contexto de Entity Framework para la base SQL Server local.
    /// </summary>
    public sealed class AppDbContext : DbContext
    {
        /// <summary>
        /// Inicializa el contexto con las opciones de SQL Server.
        /// </summary>
        /// <param name="options">Opciones de configuración del contexto.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Deportes persistidos localmente.
        /// </summary>
        public DbSet<Sport> Sports => Set<Sport>();

        /// <summary>
        /// Ligas asociadas a cada deporte, según el catálogo de deportes.
        /// </summary>
        public DbSet<SportLeague> SportLeagues => Set<SportLeague>();

        /// <summary>
        /// Ligas del catálogo independiente de SharpAPI.
        /// </summary>
        public DbSet<League> Leagues => Set<League>();

        /// <summary>
        /// Casas de apuestas persistidas localmente.
        /// </summary>
        public DbSet<Sportsbook> Sportsbooks => Set<Sportsbook>();

        /// <summary>
        /// Regiones asociadas a cada casa de apuestas.
        /// </summary>
        public DbSet<SportsbookRegion> SportsbookRegions => Set<SportsbookRegion>();

        /// <summary>
        /// Mercados persistidos localmente.
        /// </summary>
        public DbSet<Market> Markets => Set<Market>();

        /// <summary>
        /// Deportes asociados a cada mercado.
        /// </summary>
        public DbSet<MarketSport> MarketSports => Set<MarketSport>();

        /// <summary>
        /// Mejores cuotas persistidas localmente.
        /// </summary>
        public DbSet<BestOdd> BestOdds => Set<BestOdd>();

        /// <summary>
        /// Cuotas por casa asociadas a cada mejor cuota.
        /// </summary>
        public DbSet<BestOddBook> BestOddBooks => Set<BestOddBook>();

        /// <summary>
        /// Cuotas de mercado persistidas desde GET /odds.
        /// </summary>
        public DbSet<MarketOdd> MarketOdds => Set<MarketOdd>();

        /// <summary>
        /// Oportunidades de arbitraje persistidas desde GET /opportunities/arbitrage.
        /// </summary>
        public DbSet<ArbitrageOpportunity> ArbitrageOpportunities => Set<ArbitrageOpportunity>();

        /// <summary>
        /// Patas de cada oportunidad de arbitraje.
        /// </summary>
        public DbSet<ArbitrageOpportunityLeg> ArbitrageOpportunityLegs => Set<ArbitrageOpportunityLeg>();

        /// <summary>
        /// Metadatos de la última sincronización del catálogo.
        /// </summary>
        public DbSet<CatalogSyncState> CatalogSyncStates => Set<CatalogSyncState>();

        /// <inheritdoc />
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>().AreUnicode(false);
            base.ConfigureConventions(configurationBuilder);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
