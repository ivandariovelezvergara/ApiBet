const copFormatter = new Intl.NumberFormat('es-CO', {
  style: 'currency',
  currency: 'COP',
  maximumFractionDigits: 0,
})

const numberFormatter = new Intl.NumberFormat('es-CO', {
  maximumFractionDigits: 2,
})

const dateFormatter = new Intl.DateTimeFormat('es-CO', {
  dateStyle: 'short',
  timeStyle: 'short',
  timeZone: 'America/Bogota',
})

export function formatCop(value) {
  if (value == null || value === '') {
    return '—'
  }
  return copFormatter.format(Number(value))
}

export function formatNumber(value) {
  if (value == null || value === '') {
    return '—'
  }
  return numberFormatter.format(Number(value))
}

export function formatPercent(value) {
  if (value == null || value === '') {
    return '—'
  }
  return `${numberFormatter.format(Number(value))} %`
}

export function formatDate(value) {
  if (!value) {
    return '—'
  }
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) {
    return '—'
  }
  return dateFormatter.format(date)
}

export function formatBool(value) {
  return value ? 'Sí' : 'No'
}
