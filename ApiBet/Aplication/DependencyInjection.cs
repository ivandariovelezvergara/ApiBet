using Aplication.UseCases.GetArbitrage;
using Aplication.UseCases.GetBestOdds;
using Aplication.UseCases.GetLeagues;
using Aplication.UseCases.GetMarkets;
using Aplication.UseCases.GetSports;
using Aplication.UseCases.GetSportsbooks;
using Aplication.UseCases.SyncArbitrage;
using Aplication.UseCases.SyncBestOdds;
using Aplication.UseCases.SyncLeagues;
using Aplication.UseCases.SyncMarkets;
using Aplication.UseCases.SyncOdds;
using Aplication.UseCases.SyncOddsDelta;
using Aplication.UseCases.SyncSports;
using Aplication.UseCases.SyncSportsbooks;
using Microsoft.Extensions.DependencyInjection;

namespace Aplication
{
    /// <summary>
    /// Registro de los casos de uso de la capa de aplicación.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Agrega los servicios de aplicación al contenedor de dependencias.
        /// </summary>
        /// <param name="services">Colección de servicios de la aplicación.</param>
        /// <returns>La misma colección para encadenar registros.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ISyncSportsUseCase, SyncSportsUseCase>();
            services.AddScoped<IGetSportsUseCase, GetSportsUseCase>();
            services.AddScoped<ISyncLeaguesUseCase, SyncLeaguesUseCase>();
            services.AddScoped<IGetLeaguesUseCase, GetLeaguesUseCase>();
            services.AddScoped<ISyncSportsbooksUseCase, SyncSportsbooksUseCase>();
            services.AddScoped<IGetSportsbooksUseCase, GetSportsbooksUseCase>();
            services.AddScoped<ISyncMarketsUseCase, SyncMarketsUseCase>();
            services.AddScoped<IGetMarketsUseCase, GetMarketsUseCase>();
            services.AddScoped<ISyncBestOddsUseCase, SyncBestOddsUseCase>();
            services.AddScoped<IGetBestOddsUseCase, GetBestOddsUseCase>();
            services.AddScoped<ISyncOddsUseCase, SyncOddsUseCase>();
            services.AddScoped<ISyncOddsDeltaUseCase, SyncOddsDeltaUseCase>();
            services.AddScoped<ISyncArbitrageUseCase, SyncArbitrageUseCase>();
            services.AddScoped<IGetArbitrageUseCase, GetArbitrageUseCase>();
            return services;
        }
    }
}
