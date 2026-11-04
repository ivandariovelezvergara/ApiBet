function DataTable({ title, columns, rows, empty, error, compact, loading }) {
  return (
    <section className="results">
      <h2>
        {title}
        {Array.isArray(rows) ? ` · ${rows.length}` : ''}
      </h2>
      {loading && (!rows || rows.length === 0) ? <p className="banner">Cargando…</p> : null}
      {!loading && error ? <p className="banner error">{error}</p> : null}
      {!loading && !error && (!rows || rows.length === 0) ? (
        <p className="banner">{empty}</p>
      ) : null}
      {rows && rows.length > 0 ? (
        <div className={compact ? 'table-wrap compact' : 'table-wrap'}>
          <table>
            <thead>
              <tr>
                {columns.map((column) => (
                  <th key={column.key}>{column.header}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {rows.map((row, index) => (
                <tr key={row.id ?? row.pick ?? `${title}-${index}`}>
                  {columns.map((column) => (
                    <td key={column.key}>
                      {column.render ? column.render(row) : (row[column.key] ?? '—')}
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : null}
    </section>
  )
}

export default DataTable
