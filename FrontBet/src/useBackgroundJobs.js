import { useCallback, useEffect, useRef, useState } from 'react'
import {
  ARBITRAGE_ACTIVE_INTERVAL_MS,
  ARBITRAGE_SYNC_INTERVAL_MS,
  CATALOGS_SYNC_INTERVAL_MS,
  createPlacedBet,
  getBestOddsCatalog,
  getCatalogsSyncDelayMs,
  getPlacedBets,
  loadCatalogTables,
  ODDS_SYNC_INTERVAL_MS,
  runArbitrageCycle,
  runOddsCycle,
  syncReferenceCatalogs,
  updatePlacedBetStatus,
} from './api'

const idleStatus = {
  running: false,
  result: null,
  error: '',
}

const ACTIVE_OPERATION_KEY = 'frontbet.arbitrage.activeOperation'

function readActiveOperation() {
  return window.localStorage.getItem(ACTIVE_OPERATION_KEY) === '1'
}

export function useBackgroundJobs() {
  const [activeOperation, setActiveOperationState] = useState(readActiveOperation)
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
    placedBets: [],
    placedBetsSummary: null,
    catalogsError: '',
    arbitrageError: '',
    oddsError: '',
    placedBetsError: '',
    catalogsLoading: true,
    arbitrageLoading: true,
    oddsLoading: true,
    placedBetsLoading: true,
    busyBetKey: '',
  })

  const cancelledRef = useRef(false)
  const readyRef = useRef(false)
  const arbitrageTimerRef = useRef(0)
  const runArbitrageRef = useRef(async () => {})
  const scheduleArbitrageRef = useRef(() => {})
  const arbitrageIntervalMs = activeOperation
    ? ARBITRAGE_ACTIVE_INTERVAL_MS
    : ARBITRAGE_SYNC_INTERVAL_MS
  const intervalRef = useRef(arbitrageIntervalMs)

  const refreshPlacedBets = useCallback(async () => {
    try {
      const catalog = await getPlacedBets()
      if (cancelledRef.current) {
        return
      }

      setTables((current) => ({
        ...current,
        placedBets: catalog?.data ?? [],
        placedBetsSummary: catalog?.summary ?? null,
        placedBetsError: '',
        placedBetsLoading: false,
      }))
    } catch (cause) {
      if (cancelledRef.current) {
        return
      }

      setTables((current) => ({
        ...current,
        placedBetsError: cause.message || 'No se pudo cargar la bitácora de apuestas.',
        placedBetsLoading: false,
      }))
    }
  }, [])

  const registerValueBet = useCallback(async (pickRow) => {
    const key = `${pickRow.partido}|${pickRow.mercado}|${pickRow.apostarA}|${pickRow.linea ?? ''}`
    setTables((current) => ({ ...current, busyBetKey: key, placedBetsError: '' }))

    try {
      await createPlacedBet({
        betKind: 'VALUE_BET',
        deporte: pickRow.deporte,
        liga: pickRow.liga,
        partido: pickRow.partido,
        mercado: pickRow.mercado,
        seleccion: pickRow.apostarA,
        linea: pickRow.linea ?? null,
        casa: pickRow.casa,
        cuotaTomada: pickRow.cuotaActual,
        cuotaMinimaRentable: pickRow.cuotaMinimaRentable,
        probConsensoTomadaPct: pickRow.probConsensoPct,
        evEstimadoPct: pickRow.evPct,
        stakeCop: pickRow.stakeCop,
        gananciaEsperadaCop: pickRow.gananciaEsperadaCop,
      })

      const [betsCatalog, oddsCatalog] = await Promise.all([
        getPlacedBets(),
        getBestOddsCatalog(),
      ])

      if (cancelledRef.current) {
        return
      }

      setTables((current) => ({
        ...current,
        placedBets: betsCatalog?.data ?? [],
        placedBetsSummary: betsCatalog?.summary ?? null,
        picks: oddsCatalog?.picks ?? current.picks,
        summaries: oddsCatalog?.summaries ?? current.summaries,
        placedBetsError: '',
        placedBetsLoading: false,
        busyBetKey: '',
      }))
    } catch (cause) {
      if (cancelledRef.current) {
        return
      }

      setTables((current) => ({
        ...current,
        placedBetsError: cause.message || 'No se pudo registrar la apuesta.',
        busyBetKey: '',
      }))
    }
  }, [])

  const changeBetStatus = useCallback(async (id, estado) => {
    const key = `status-${id}`
    setTables((current) => ({ ...current, busyBetKey: key, placedBetsError: '' }))

    try {
      await updatePlacedBetStatus(id, estado)
      const [betsCatalog, oddsCatalog] = await Promise.all([
        getPlacedBets(),
        getBestOddsCatalog(),
      ])

      if (cancelledRef.current) {
        return
      }

      setTables((current) => ({
        ...current,
        placedBets: betsCatalog?.data ?? [],
        placedBetsSummary: betsCatalog?.summary ?? null,
        picks: oddsCatalog?.picks ?? current.picks,
        summaries: oddsCatalog?.summaries ?? current.summaries,
        placedBetsError: '',
        busyBetKey: '',
      }))
    } catch (cause) {
      if (cancelledRef.current) {
        return
      }

      setTables((current) => ({
        ...current,
        placedBetsError: cause.message || 'No se pudo actualizar el estado de la apuesta.',
        busyBetKey: '',
      }))
    }
  }, [])

  const runArbitrage = useCallback(async () => {
    setArbitrage((current) => ({ ...current, running: true, error: '' }))
    setTables((current) => ({ ...current, arbitrageLoading: true, arbitrageError: '' }))

    try {
      const { sync, catalog, syncError } = await runArbitrageCycle()
      if (cancelledRef.current) {
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
      if (cancelledRef.current) {
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
  }, [])

  useEffect(() => {
    runArbitrageRef.current = runArbitrage
    intervalRef.current = arbitrageIntervalMs
  }, [runArbitrage, arbitrageIntervalMs])

  const scheduleArbitrage = useCallback(() => {
    window.clearTimeout(arbitrageTimerRef.current)
    arbitrageTimerRef.current = window.setTimeout(async () => {
      await runArbitrageRef.current()
      if (!cancelledRef.current) {
        scheduleArbitrageRef.current()
      }
    }, intervalRef.current)
  }, [])

  useEffect(() => {
    scheduleArbitrageRef.current = scheduleArbitrage
  }, [scheduleArbitrage])

  const syncArbitrageNow = useCallback(async () => {
    window.clearTimeout(arbitrageTimerRef.current)
    await runArbitrageRef.current()
    if (!cancelledRef.current && readyRef.current) {
      scheduleArbitrage()
    }
  }, [scheduleArbitrage])

  function setActiveOperation(next) {
    window.localStorage.setItem(ACTIVE_OPERATION_KEY, next ? '1' : '0')
    setActiveOperationState(next)
  }

  useEffect(() => {
    let cancelled = false
    cancelledRef.current = false
    let catalogsTimer
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
        await refreshPlacedBets()
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
      refreshPlacedBets()
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

      async function oddsLoop() {
        await runOdds()
        if (!cancelled) {
          oddsTimer = window.setTimeout(oddsLoop, ODDS_SYNC_INTERVAL_MS)
        }
      }

      readyRef.current = true
      await runArbitrageRef.current()
      if (!cancelled) {
        scheduleArbitrage()
      }
      oddsLoop()
    }

    start()

    return () => {
      cancelled = true
      cancelledRef.current = true
      readyRef.current = false
      window.clearTimeout(catalogsTimer)
      window.clearTimeout(arbitrageTimerRef.current)
      window.clearTimeout(oddsTimer)
    }
  }, [refreshPlacedBets, scheduleArbitrage])

  const skipIntervalReset = useRef(true)
  useEffect(() => {
    if (skipIntervalReset.current) {
      skipIntervalReset.current = false
      return
    }
    if (!readyRef.current || cancelledRef.current) {
      return
    }

    if (activeOperation) {
      syncArbitrageNow()
      return
    }

    scheduleArbitrage()
  }, [activeOperation, scheduleArbitrage, syncArbitrageNow])

  return {
    catalogs,
    arbitrage,
    odds,
    tables,
    activeOperation,
    arbitrageIntervalMs,
    setActiveOperation,
    syncArbitrageNow,
    registerValueBet,
    changeBetStatus,
  }
}
