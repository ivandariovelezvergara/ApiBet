import ResultTables from './ResultTables'
import { formatDate } from './format'
import { useBackgroundJobs } from './useBackgroundJobs'
import './App.css'

function App() {
  const { catalogs, arbitrage, odds, tables } = useBackgroundJobs()

  return (
    <main className="page">
      <header className="hero">
        <p className="eyebrow">ApiBet</p>
        <h1>Oportunidades</h1>
        <div className="sync-status">
          <p>{formatCatalogsSync(catalogs)}</p>
          <p>{formatArbitrageSync(arbitrage)}</p>
          <p>{formatOddsSync(odds)}</p>
        </div>
      </header>

      <ResultTables tables={tables} />
    </main>
  )
}

function formatCatalogsSync(sync) {
  if (sync.running) {
    return 'Catálogos: sincronizando sports, sportsbooks, markets y leagues…'
  }
  if (sync.error) {
    return `Catálogos: ${sync.error}`
  }
  if (sync.result) {
    const lastAt =
      sync.result.leagues?.lastSyncedAtUtc ??
      sync.result.markets?.lastSyncedAtUtc ??
      sync.result.sportsbooks?.lastSyncedAtUtc ??
      sync.result.sports?.lastSyncedAtUtc
    const sports = sync.result.sports?.data?.length ?? 0
    const sportsbooks = sync.result.sportsbooks?.data?.length ?? 0
    const markets = sync.result.markets?.data?.length ?? 0
    const leagues = sync.result.leagues?.data?.length ?? 0
    return `Catálogos: sports ${sports} · casas ${sportsbooks} · mercados ${markets} · ligas ${leagues} · ${formatDate(lastAt)}`
  }
  return 'Catálogos: pendiente del arranque.'
}

function formatArbitrageSync(sync) {
  if (sync.running) {
    return 'Arbitraje: sincronizando en segundo plano…'
  }
  if (sync.error) {
    return `Arbitraje: ${sync.error}`
  }
  if (sync.result?.lastSyncedAtUtc) {
    const count = sync.result.storedCount
    return `Arbitraje: última sync ${formatDate(sync.result.lastSyncedAtUtc)}${
      count != null ? ` · ${count} filas` : ''
    }`
  }
  return 'Arbitraje: espera a que terminen los catálogos.'
}

function formatOddsSync(sync) {
  if (sync.running) {
    return 'Odds: sincronizando delta y luego best odds…'
  }
  if (sync.error) {
    return `Odds: ${sync.error}`
  }
  if (sync.result) {
    const deltaAt = sync.result.delta?.lastSyncedAtUtc
    const bestAt = sync.result.best?.lastSyncedAtUtc
    const deltaCount = sync.result.delta?.upsertedCount
    const bestCount = sync.result.best?.storedCount
    return `Odds: delta ${formatDate(deltaAt)} (${deltaCount ?? 0}) · best ${formatDate(bestAt)} (${bestCount ?? 0})`
  }
  return 'Odds: espera a que termine el arbitraje.'
}

export default App
