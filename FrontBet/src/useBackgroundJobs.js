import { useEffect, useState } from 'react'
import {
  ARBITRAGE_SYNC_INTERVAL_MS,
  CATALOGS_SYNC_INTERVAL_MS,
  getCatalogsSyncDelayMs,
  loadCatalogTables,
  ODDS_SYNC_INTERVAL_MS,
  runArbitrageCycle,
  runOddsCycle,
  syncReferenceCatalogs,
} from './api'

const idleStatus = {
  running: false,
  result: null,
  error: '',
}

export function useBackgroundJobs() {
  const [catalogs, setCatalogs] = useState(idleStatus)
  const [arbitrage, setArbitrage] = useState(idleStatus)
  const [odds, setOdds] = useState(idleStatus)
  const [tables, setTables] = useState({
    sports: [],
    sportsbooks: [],
    markets: [],
    leagues: [],
    arbitrageRows: [],
    summaries: [],
    picks: [],
    catalogsError: '',
    arbitrageError: '',
    oddsError: '',
    catalogsLoading: true,
    arbitrageLoading: true,
    oddsLoading: true,
  })

  useEffect(() => {
    let cancelled = false
    let catalogsTimer
    let arbitrageTimer
    let oddsTimer

    async function runCatalogs() {
      setCatalogs((current) => ({ ...current, running: true, error: '' }))
      setTables((current) => ({ ...current, catalogsLoading: true, catalogsError: '' }))

      try {
        let result = null
        if (getCatalogsSyncDelayMs() === 0) {
          result = await syncReferenceCatalogs()
        }

        const catalogsData = await loadCatalogTables()
        if (cancelled) {
          return
        }

        setTables((current) => ({
          ...current,
          sports: catalogsData.sports?.data ?? [],
          sportsbooks: catalogsData.sportsbooks?.data ?? [],
          markets: catalogsData.markets?.data ?? [],
          leagues: catalogsData.leagues?.data ?? [],
          catalogsError: '',
          catalogsLoading: false,
        }))
        setCatalogs({
          running: false,
          result: result ?? catalogsData,
          error: '',
        })
      } catch (cause) {
        if (cancelled) {
          return
        }

        const message = cause.message || 'No se pudieron cargar los catálogos.'
        setTables((current) => ({
          ...current,
          catalogsError: message,
          catalogsLoading: false,
        }))
        setCatalogs((current) => ({
          ...current,
          running: false,
          error: message,
        }))
      }
    }

    async function runArbitrage() {
      setArbitrage((current) => ({ ...current, running: true, error: '' }))
      setTables((current) => ({ ...current, arbitrageLoading: true, arbitrageError: '' }))

      try {
        const { sync, catalog, syncError } = await runArbitrageCycle()
        if (cancelled) {
          return
        }

        const message = syncError?.message || ''
        setTables((current) => ({
          ...current,
          arbitrageRows: catalog?.data ?? [],
          arbitrageError: message,
          arbitrageLoading: false,
        }))
        setArbitrage({
          running: false,
          result: sync,
          error: message,
        })
      } catch (cause) {
        if (cancelled) {
          return
        }

        const message = cause.message || 'No se pudo cargar el arbitraje.'
        setTables((current) => ({
          ...current,
          arbitrageError: message,
          arbitrageLoading: false,
        }))
        setArbitrage((current) => ({
          ...current,
          running: false,
          error: message,
        }))
      }
    }

    async function runOdds() {
      setOdds((current) => ({ ...current, running: true, error: '' }))
      setTables((current) => ({ ...current, oddsLoading: true, oddsError: '' }))

      try {
        const { delta, best, catalog, error } = await runOddsCycle()
        if (cancelled) {
          return
        }

        setTables((current) => ({
          ...current,
          summaries: catalog?.summaries ?? [],
          picks: catalog?.picks ?? [],
          oddsError: error,
          oddsLoading: false,
        }))
        setOdds({
          running: false,
          result: { delta, best },
          error,
        })
      } catch (cause) {
        if (cancelled) {
          return
        }

        const message = cause.message || 'No se pudieron cargar las value bets.'
        setTables((current) => ({
          ...current,
          oddsError: message,
          oddsLoading: false,
        }))
        setOdds((current) => ({
          ...current,
          running: false,
          error: message,
        }))
      }
    }

    async function start() {
      await runCatalogs()
      if (cancelled) {
        return
      }

      const catalogsDelay = getCatalogsSyncDelayMs() || CATALOGS_SYNC_INTERVAL_MS
      catalogsTimer = window.setTimeout(async function catalogsLoop() {
        await runCatalogs()
        if (!cancelled) {
          catalogsTimer = window.setTimeout(catalogsLoop, CATALOGS_SYNC_INTERVAL_MS)
        }
      }, catalogsDelay)

      async function arbitrageLoop() {
        await runArbitrage()
        if (!cancelled) {
          arbitrageTimer = window.setTimeout(arbitrageLoop, ARBITRAGE_SYNC_INTERVAL_MS)
        }
      }

      async function oddsLoop() {
        await runOdds()
        if (!cancelled) {
          oddsTimer = window.setTimeout(oddsLoop, ODDS_SYNC_INTERVAL_MS)
        }
      }

      arbitrageLoop()
      oddsLoop()
    }

    start()

    return () => {
      cancelled = true
      window.clearTimeout(catalogsTimer)
      window.clearTimeout(arbitrageTimer)
      window.clearTimeout(oddsTimer)
    }
  }, [])

  return { catalogs, arbitrage, odds, tables }
}
