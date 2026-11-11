import DataTable from './DataTable'
import {
  formatBool,
  formatCop,
  formatDate,
  formatNumber,
  formatPercent,
} from './format'

const arbitrageColumns = [
  { key: 'deporte', header: 'Deporte' },
  { key: 'league', header: 'Liga' },
  { key: 'partido', header: 'Partido' },
  { key: 'eventStartTime', header: 'Inicio', render: (row) => formatDate(row.eventStartTime) },
  { key: 'enVivo', header: 'En vivo', render: (row) => formatBool(row.enVivo) },
  { key: 'createdAtUtc', header: 'Creado', render: (row) => formatDate(row.createdAtUtc) },
  { key: 'betType', header: 'Mercado' },
  { key: 'casaApuesta1', header: 'Casa 1' },
  { key: 'equipo1', header: 'Selección 1' },
  { key: 'cuota1', header: 'Cuota 1', render: (row) => formatNumber(row.cuota1) },
  { key: 'monto1', header: 'Monto 1', render: (row) => formatCop(row.monto1) },
  { key: 'casaApuesta2', header: 'Casa 2' },
  { key: 'equipo2', header: 'Selección 2' },
  { key: 'cuota2', header: 'Cuota 2', render: (row) => formatNumber(row.cuota2) },
  { key: 'monto2', header: 'Monto 2', render: (row) => formatCop(row.monto2) },
  { key: 'banca', header: 'Banca', render: (row) => formatCop(row.banca) },
  { key: 'gananciaEsperada', header: 'Ganancia', render: (row) => formatCop(row.gananciaEsperada) },
  { key: 'totalNetoGanado', header: 'Total neto', render: (row) => formatCop(row.totalNetoGanado) },
  { key: 'instruccion', header: 'Instrucción' },
]

function buildValueBetColumns(placedBets, busyBetKey, onRegister) {
  const pendingSet = new Set(
    (placedBets ?? [])
      .filter((b) => b.estado === 'PENDING')
      .map((b) => b.partido)
  )

  return [
    {
      key: 'accion',
      header: 'Acción',
      render: (row) => {
        const rowKey = `${row.partido}|${row.mercado}|${row.apostarA}|${row.linea ?? ''}`
        const alreadyPending = pendingSet.has(row.partido)
        const isBusy = busyBetKey === rowKey

        return (
          <button
            type="button"
            className={alreadyPending ? 'table-btn done' : 'table-btn primary'}
            disabled={alreadyPending || isBusy}
            onClick={() => onRegister(row)}
          >
            {alreadyPending ? 'Registrada ✓' : isBusy ? 'Guardando…' : 'Apostar / Registrar'}
          </button>
        )
      },
    },
    { key: 'senal', header: 'Señal' },
    { key: 'deporte', header: 'Deporte' },
    { key: 'liga', header: 'Liga' },
    { key: 'partido', header: 'Partido' },
    { key: 'mercado', header: 'Mercado' },
    { key: 'apostarA', header: 'Apostar a' },
    { key: 'casa', header: 'Casa' },
    { key: 'cuotaActual', header: 'Cuota actual', render: (row) => formatNumber(row.cuotaActual) },
    { key: 'cuotaMinimaRentable', header: 'Cuota mínima rentable', render: (row) => formatNumber(row.cuotaMinimaRentable) },
    { key: 'probConsensoPct', header: 'Prob. consenso', render: (row) => formatPercent(row.probConsensoPct) },
    { key: 'probImplicitaPct', header: 'Prob. implícita', render: (row) => formatPercent(row.probImplicitaPct) },
    { key: 'instruccion', header: 'Instrucción' },
  ]
}

function buildPlacedBetColumns(busyBetKey, onChangeStatus) {
  return [
    { key: 'placedAtCol', header: 'Fecha (COL)', render: (row) => formatDate(row.placedAtCol) },
    { key: 'deporte', header: 'Deporte' },
    { key: 'liga', header: 'Liga' },
    { key: 'partido', header: 'Partido' },
    {
      key: 'mercado',
      header: 'Mercado / Línea',
      render: (row) =>
        row.linea != null ? `${row.mercado} (${formatNumber(row.linea)})` : row.mercado,
    },
    { key: 'seleccion', header: 'Apostado a' },
    { key: 'casa', header: 'Casa' },
    { key: 'cuotaTomada', header: 'Cuota tomada', render: (row) => formatNumber(row.cuotaTomada) },
    {
      key: 'ultimaCuotaMercado',
      header: 'Cuota cierre',
      render: (row) => formatNumber(row.ultimaCuotaMercado),
    },
    {
      key: 'clvCuotaPct',
      header: 'CLV %',
      render: (row) => formatPercent(row.clvCuotaPct),
    },
    { key: 'evEstimadoPct', header: 'EV est. %', render: (row) => formatPercent(row.evEstimadoPct) },
    { key: 'stakeCop', header: 'Stake', render: (row) => formatCop(row.stakeCop) },
    { key: 'estado', header: 'Estado', render: (row) => formatStatusBadge(row.estado) },
    {
      key: 'profitRealCop',
      header: 'P&L Real',
      render: (row) => (row.profitRealCop != null ? formatCop(row.profitRealCop) : '—'),
    },
    {
      key: 'liquidar',
      header: 'Liquidar resultado',
      render: (row) => {
        const isBusy = busyBetKey === `status-${row.id}`
        return (
          <div className="row-actions">
            <button
              type="button"
              className={row.estado === 'WON' ? 'table-btn won active' : 'table-btn won'}
              disabled={isBusy || row.estado === 'WON'}
              onClick={() => onChangeStatus(row.id, 'WON')}
            >
              Ganada
            </button>
            <button
              type="button"
              className={row.estado === 'LOST' ? 'table-btn lost active' : 'table-btn lost'}
              disabled={isBusy || row.estado === 'LOST'}
              onClick={() => onChangeStatus(row.id, 'LOST')}
            >
              Perdida
            </button>
            <button
              type="button"
              className={row.estado === 'PUSH' ? 'table-btn push active' : 'table-btn push'}
              disabled={isBusy || row.estado === 'PUSH'}
              onClick={() => onChangeStatus(row.id, 'PUSH')}
            >
              Push
            </button>
            {row.estado !== 'PENDING' ? (
              <button
                type="button"
                className="table-btn"
                disabled={isBusy}
                onClick={() => onChangeStatus(row.id, 'PENDING')}
              >
                Reabrir
              </button>
            ) : null}
          </div>
        )
      },
    },
  ]
}

function formatStatusBadge(status) {
  switch (status) {
    case 'WON':
      return '✅ GANADA'
    case 'LOST':
      return '❌ PERDIDA'
    case 'PUSH':
      return '➖ PUSH'
    case 'VOID':
      return '⚪ NULA'
    default:
      return '⏳ PENDIENTE'
  }
}

function PlacedBetsSummaryBar({ summary }) {
  if (!summary) {
    return null
  }

  return (
    <div className="clv-summary">
      <span>
        <strong>Apuestas:</strong> {summary.totalBets} ({summary.pendingBets} pendientes ·{' '}
        {summary.wonBets}W / {summary.lostBets}L / {summary.pushBets}P)
      </span>
      <span>
        <strong>Exposición activa:</strong> {formatCop(summary.pendingExposureCop)}
      </span>
      <span>
        <strong>P&amp;L Realizado:</strong> {formatCop(summary.realizedProfitCop)} (ROI{' '}
        {formatPercent(summary.roiPct)})
      </span>
      <span>
        <strong>EV Medio:</strong> {formatPercent(summary.avgEvPct)}
      </span>
      <span>
        <strong>CLV Medio:</strong> {formatPercent(summary.avgClvPct)} (Bate cierre:{' '}
        {formatPercent(summary.positiveClvRatePct)})
      </span>
    </div>
  )
}

function ResultTables({ tables, onRegisterValueBet, onChangeBetStatus }) {
  const valueBetColumns = buildValueBetColumns(
    tables.placedBets,
    tables.busyBetKey,
    onRegisterValueBet
  )
  const placedBetColumns = buildPlacedBetColumns(tables.busyBetKey, onChangeBetStatus)

  return (
    <>
      <DataTable
        title="Arbitraje"
        columns={arbitrageColumns}
        rows={tables.arbitrageRows}
        empty="No hay boletas de arbitraje."
        error={tables.arbitrageError}
        loading={tables.arbitrageLoading}
      />
      <DataTable
        title="Value bets"
        columns={valueBetColumns}
        rows={tables.picks}
        empty="No hay value bets."
        error={tables.oddsError}
        loading={tables.oddsLoading}
      />
      <section className="results">
        <PlacedBetsSummaryBar summary={tables.placedBetsSummary} />
        <DataTable
          title="Bitácora de apuestas ejecutadas (CLV & ROI)"
          columns={placedBetColumns}
          rows={tables.placedBets}
          empty="Aún no has registrado apuestas ejecutadas."
          error={tables.placedBetsError}
          loading={tables.placedBetsLoading}
        />
      </section>
    </>
  )
}

export default ResultTables
