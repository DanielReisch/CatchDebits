import type { Pessoa } from '../../types';

interface Props {
  pessoas: Pessoa[];
  onDeletar: (id: number) => void;
}

export function PessoaList({ pessoas, onDeletar }: Props) {
  if (pessoas.length === 0)
    return <p style={{ color: '#6c7086' }}>Nenhuma pessoa cadastrada.</p>;

  return (
    <table style={styles.table}>
      <thead>
        <tr>
          <th style={styles.th}>ID</th>
          <th style={styles.th}>Nome</th>
          <th style={styles.th}>Idade</th>
          <th style={styles.th}>Ações</th>
        </tr>
      </thead>
      <tbody>
        {pessoas.map(p => (
          <tr key={p.id} style={styles.tr}>
            <td style={styles.td}>{p.id}</td>
            <td style={styles.td}>{p.nome}</td>
            <td style={styles.td}>
              {p.idade} anos
              {p.idade < 18 && <span style={styles.badge}> ⚠️ Menor</span>}
            </td>
            <td style={styles.td}>
              <button style={styles.btnDel} onClick={() => onDeletar(p.id)}>
                🗑️ Deletar
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

const styles: Record<string, React.CSSProperties> = {
  table:  { width: '100%', borderCollapse: 'collapse', marginTop: 16 },
  th:     { textAlign: 'left', padding: '10px 12px', background: '#313244', color: '#89b4fa', fontWeight: 600 },
  tr:     { borderBottom: '1px solid #313244' },
  td:     { padding: '10px 12px', color: '#cdd6f4' },
  badge:  { color: '#fab387', fontSize: 12 },
  btnDel: { padding: '4px 10px', borderRadius: 4, border: 'none', background: '#f38ba8', color: '#1e1e2e', cursor: 'pointer', fontWeight: 'bold' },
};