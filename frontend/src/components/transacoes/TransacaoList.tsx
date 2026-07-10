import { TipoTransacao } from '../../types';
import type { Transacao } from '../../types';

interface Props {
  transacoes: Transacao[];
}

export function TransacaoList({ transacoes }: Props) {
  if (transacoes.length === 0)
    return <p style={{ color: '#6c7086' }}>Nenhuma transação cadastrada.</p>;

  const fmt = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

  return (
    <table style={styles.table}>
      <thead>
        <tr>
          <th style={styles.th}>Descrição</th>
          <th style={styles.th}>Valor</th>
          <th style={styles.th}>Tipo</th>
          <th style={styles.th}>Pessoa</th>
        </tr>
      </thead>
      <tbody>
        {transacoes.map(t => (
          <tr key={t.id} style={styles.tr}>
            <td style={styles.td}>{t.descricao}</td>
            <td style={{ ...styles.td, color: t.tipo === TipoTransacao.Receita ? '#a6e3a1' : '#f38ba8', fontWeight: 600 }}>
              {fmt(t.valor)}
            </td>
            <td style={styles.td}>
              <span style={{ ...styles.tag, background: t.tipo === TipoTransacao.Receita ? '#a6e3a1' : '#f38ba8', color: '#1e1e2e' }}>
                {t.tipoDescricao}
              </span>
            </td>
            <td style={styles.td}>{t.nomePessoa}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

const styles: Record<string, React.CSSProperties> = {
  table: { width: '100%', borderCollapse: 'collapse', marginTop: 16 },
  th:    { textAlign: 'left', padding: '10px 12px', background: '#313244', color: '#89b4fa', fontWeight: 600 },
  tr:    { borderBottom: '1px solid #313244' },
  td:    { padding: '10px 12px', color: '#cdd6f4' },
  tag:   { padding: '2px 8px', borderRadius: 12, fontSize: 12, fontWeight: 'bold' },
};