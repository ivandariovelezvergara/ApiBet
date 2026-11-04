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

const valueBetColumns = [
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

function ResultTables({ tables }) {
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
    </>
  )
}

export default ResultTables
