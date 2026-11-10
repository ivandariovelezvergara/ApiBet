import { config } from './config'

const inFlightByPath = new Map()
let oddsCycleInFlight = null
let catalogsPipelineInFlight = null
let arbitrageCycleInFlight = null

const CATALOGS_SYNC_KEY = 'frontbet.catalogs.lastSyncAt'
export const CATALOGS_SYNC_INTERVAL_MS = config.catalogs.intervalMs
export const ARBITRAGE_SYNC_INTERVAL_MS = config.arbitrage.intervalMs
export const ARBITRAGE_ACTIVE_INTERVAL_MS = config.arbitrage.activeIntervalMs
export const ODDS_SYNC_INTERVAL_MS = config.odds.intervalMs

export function getJson(path) {
  return fetch(path).then(readJson)
}

export function runArbitrageCycle() {
  if (!arbitrageCycleInFlight) {
    arbitrageCycleInFlight = executeArbitrageCycle().finally(() => {
      arbitrageCycleInFlight = null
    })
  }

  return arbitrageCycleInFlight
}

async function executeArbitrageCycle() {
  let sync = null
  let syncError = null

  try {
    sync = await postJson(config.arbitrage.syncPath)
  } catch (error) {
    syncError = error
  }

  const catalog = await getJson(config.arbitrage.getPath)
  return { sync, catalog, syncError }
}

export function runOddsCycle() {
  if (!oddsCycleInFlight) {
    oddsCycleInFlight = executeOddsCycle().finally(() => {
      oddsCycleInFlight = null
    })
  }

  return oddsCycleInFlight
}

async function executeOddsCycle() {
  let delta = null
  let best = null
  const errors = []

  try {
    delta = await postJson(config.odds.deltaSyncPath)
  } catch (error) {
    errors.push(error.message || 'Falló delta/sync.')
  }

  try {
    best = await postJson(config.odds.bestSyncPath)
  } catch (error) {
    errors.push(error.message || 'Falló best/sync.')
  }

  const catalog = await getJson(config.odds.getPath)
  return {
    delta,
    best,
    catalog,
    error: errors.join(' · '),
  }
}

export function getCatalogsSyncDelayMs() {
  const last = Number(window.localStorage.getItem(CATALOGS_SYNC_KEY) || 0)
  if (!last) {
    return 0
  }

  return Math.max(0, CATALOGS_SYNC_INTERVAL_MS - (Date.now() - last))
}

export function syncReferenceCatalogs() {
  if (!catalogsPipelineInFlight) {
    catalogsPipelineInFlight = (async () => {
      const sports = await postJson(config.catalogs.sportsSyncPath)
      const sportsbooks = await postJson(config.catalogs.sportsbooksSyncPath)
      const markets = await postJson(config.catalogs.marketsSyncPath)
      const leagues = await postJson(config.catalogs.leaguesSyncPath)
      window.localStorage.setItem(CATALOGS_SYNC_KEY, String(Date.now()))
      return { sports, sportsbooks, markets, leagues }
    })().finally(() => {
      catalogsPipelineInFlight = null
    })
  }

  return catalogsPipelineInFlight
}

export async function loadCatalogTables() {
  const [sports, sportsbooks, markets, leagues] = await Promise.all([
    getJson(config.catalogs.sportsGetPath),
    getJson(config.catalogs.sportsbooksGetPath),
    getJson(config.catalogs.marketsGetPath),
    getJson(config.catalogs.leaguesGetPath),
  ])

  return { sports, sportsbooks, markets, leagues }
}

function postJson(path) {
  const existing = inFlightByPath.get(path)
  if (existing) {
    return existing
  }

  const request = fetch(path, { method: 'POST' })
    .then(readJson)
    .finally(() => {
      inFlightByPath.delete(path)
    })

  inFlightByPath.set(path, request)
  return request
}

async function readJson(response) {
  if (!response.ok) {
    let detail = `Error ${response.status}`
    try {
      const body = await response.json()
      detail = body.message ?? body.detail ?? detail
    } catch {
      // Se conserva el mensaje HTTP si el cuerpo no es JSON.
    }
    throw new Error(detail)
  }

  return response.json()
}
