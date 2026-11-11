function textEnv(name, fallback) {
  const value = import.meta.env[name]
  return value && String(value).trim() ? String(value).trim() : fallback
}

function numberEnv(name, fallback) {
  const parsed = Number(import.meta.env[name])
  return Number.isFinite(parsed) && parsed > 0 ? parsed : fallback
}

export const config = {
  apiProxyTarget: textEnv('VITE_API_PROXY_TARGET', 'http://localhost:5035'),
  arbitrage: {
    syncPath: textEnv('VITE_ARBITRAGE_SYNC_PATH', '/api/arbitrage/sync'),
    getPath: textEnv('VITE_ARBITRAGE_GET_PATH', '/api/arbitrage'),
    intervalMs: numberEnv('VITE_ARBITRAGE_INTERVAL_MINUTES', 3) * 60 * 1000,
    activeIntervalMs: numberEnv('VITE_ARBITRAGE_ACTIVE_INTERVAL_SECONDS', 60) * 1000,
  },
  odds: {
    deltaSyncPath: textEnv('VITE_ODDS_DELTA_SYNC_PATH', '/api/odds/delta/sync'),
    bestSyncPath: textEnv('VITE_ODDS_BEST_SYNC_PATH', '/api/odds/best/sync'),
    getPath: textEnv('VITE_ODDS_GET_PATH', '/api/odds/best'),
    intervalMs: numberEnv('VITE_ODDS_INTERVAL_MINUTES', 10) * 60 * 1000,
  },
  bets: {
    path: textEnv('VITE_BETS_PATH', '/api/bets'),
  },
  catalogs: {
    sportsSyncPath: textEnv('VITE_SPORTS_SYNC_PATH', '/api/sports/sync'),
    sportsbooksSyncPath: textEnv('VITE_SPORTSBOOKS_SYNC_PATH', '/api/sportsbooks/sync'),
    marketsSyncPath: textEnv('VITE_MARKETS_SYNC_PATH', '/api/markets/sync'),
    leaguesSyncPath: textEnv('VITE_LEAGUES_SYNC_PATH', '/api/leagues/sync'),
    sportsGetPath: textEnv('VITE_SPORTS_GET_PATH', '/api/sports'),
    sportsbooksGetPath: textEnv('VITE_SPORTSBOOKS_GET_PATH', '/api/sportsbooks'),
    marketsGetPath: textEnv('VITE_MARKETS_GET_PATH', '/api/markets'),
    leaguesGetPath: textEnv('VITE_LEAGUES_GET_PATH', '/api/leagues'),
    intervalMs: numberEnv('VITE_CATALOGS_INTERVAL_HOURS', 24) * 60 * 60 * 1000,
  },
}
