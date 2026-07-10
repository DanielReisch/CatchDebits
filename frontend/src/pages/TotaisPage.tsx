import { useState, useEffect } from 'react';
import type { RelatorioTotais } from '../types';
import { getRelatorioTotais } from '../services/transacaoService';

const fmt = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

export function TotaisPage() {
  const [relatorio, setRelatorio] = useState<RelatorioTotais | null>(null);

  useEffect(() => { getRelatorioTotais().then(setRelatorio); }, []);

  if (!relatorio) return <p style={{ color: '#cdd6f4' }}>Carregando relatório...</p>;

  return (
    <div style={styles.container}>
      <h2 style={styles.titulo}>📊 Relatório de Totais</h2>
      <table style={styles.table}>
        <thead>
          <tr>
            {['Pessoa','Idade','Total Receitas','Total Despesas','Saldo Líquido'].map(h => (
              <th key={h} style={styles.th}>{h}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {relatorio.totaisPorPessoa.map(t => (
            <tr key={t.pessoaId} style={styles.tr}>
              <td style={styles.td}>{t.nomePessoa}</td>
              <td style={styles.td}>{t.idadePessoa} anos</td>
              <td style={{ ...styles.td, color: '#a6e3a1' }}>{fmt(t.totalReceitas)}</td>
              <td style={{ ...styles.td, color: '#f38ba8' }}>{fmt(t.totalDespesas)}</td>
              <td style={{ ...styles.td, color: t.saldoLiquido >= 0 ? '#a6e3a1' : '#f38ba8', fontWeight: 'bold' }}>
                {fmt(t.saldoLiquido)}
              </td>
            </tr>
          ))}
        </tbody>
        <tfoot>
          <tr style={{ background: '#313244' }}>
            <td style={{ ...styles.td, fontWeight: 'bold', color: '#89b4fa' }} colSpan={2}>TOTAL GERAL</td>
            <td style={{ ...styles.td, color: '#a6e3a1', fontWeight: 'bold' }}>{fmt(relatorio.totalReceitasGeral)}</td>
            <td style={{ ...styles.td, color: '#f38ba8', fontWeight: 'bold' }}>{fmt(relatorio.totalDespesasGeral)}</td>
            <td style={{ ...styles.td, fontWeight: 'bold', color: relatorio.saldoLiquidoGeral >= 0 ? '#a6e3a1' : '#f38ba8' }}>
              {fmt(relatorio.saldoLiquidoGeral)}
            </td>
          </tr>
        </tfoot>
      </table>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  container: { maxWidth: 1000, margin: '0 auto' },
  titulo:    { color: '#cdd6f4', marginBottom: 24 },
  table:     { width: '100%', borderCollapse: 'collapse' },
  th:        { textAlign: 'left', padding: '10px 12px', background: '#313244', color: '#89b4fa', fontWeight: 600 },
  tr:        { borderBottom: '1px solid #313244' },
  td:        { padding: '10px 12px', color: '#cdd6f4' },
};