using Domain.Interfaces;
using Infraestructure.External.SharpApi;
using Infraestructure.Persistence;
using Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infraestructure
{
    /// <summary>
    /// Registro de persistencia SQL Server y del cliente HTTP de SharpAPI.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Agrega los adaptadores de infraestructura al contenedor de dependencias.
        /// </summary>
        /// <param name="services">Colección de servicios de la aplicación.</param>
        /// <param name="configuration">Configuración de la aplicación anfitriona.</param>
        /// <returns>La misma colección para encadenar registros.</returns>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<SharpApiOptions>()
                .Bind(configuration.GetSection(SharpApiOptions.SectionName))
                .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), "SharpApi:ApiKey is required.")
                .Validate(
                    options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
                    "SharpApi:BaseUrl must be an absolute URI.")
                .ValidateOnStart();

            var connectionString = configuration.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("ConnectionStrings:SqlServer is required.");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<ISportRepository, SportRepository>();
            services.AddScoped<ILeagueRepository, LeagueRepository>();
            services.AddScoped<ISportsbookRepository, SportsbookRepository>();
            services.AddScoped<IMarketRepository, MarketRepository>();
            services.AddScoped<IBestOddsRepository, BestOddsRepository>();
            services.AddScoped<IOddsRepository, OddsRepository>();
            services.AddScoped<IArbitrageOpportunityRepository, ArbitrageOpportunityRepository>();
            services.AddScoped<IPlacedBetRepository, PlacedBetRepository>();
            services.AddScoped<ISportCatalogClient, SharpApiSportCatalogClient>();
            services.AddScoped<ILeagueCatalogClient, SharpApiLeagueCatalogClient>();
            services.AddScoped<ISportsbookCatalogClient, SharpApiSportsbookCatalogClient>();
            services.AddScoped<IMarketCatalogClient, SharpApiMarketCatalogClient>();
            services.AddScoped<IBestOddsClient, SharpApiBestOddsClient>();
            services.AddScoped<IOddsClient, SharpApiOddsClient>();
            services.AddScoped<IArbitrageClient, SharpApiArbitrageClient>();

            services.AddHttpClient<SharpApiHttpGateway>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<SharpApiOptions>>().Value;
                var baseUrl = options.BaseUrl.EndsWith('/') ? options.BaseUrl : $"{options.BaseUrl}/";

                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.Timeout = TimeSpan.FromSeconds(90);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-API-Key", options.ApiKey);
            });

            return services;
        }
    }
}
