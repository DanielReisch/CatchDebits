import { useState, useEffect, useCallback } from 'react';
import type { Transacao } from '../types';
import { getTransacoes } from '../services/transacaoService';
import { TransacaoForm } from '../components/transacoes/TransacaoForm';
import { TransacaoList } from '../components/transacoes/TransacaoList';

export function TransacoesPage() {
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);

  const carregar = useCallback(async () => {
    const data = await getTransacoes();
    setTransacoes(data);
  }, []);

  useEffect(() => { carregar(); }, [carregar]);

  return (
    <div style={styles.container}>
      <h2 style={styles.titulo}>💸 Transações</h2>
      <div style={styles.grid}>
        <TransacaoForm onSuccess={carregar} />
        <TransacaoList transacoes={transacoes} />
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  container: { maxWidth: 1100, margin: '0 auto' },
  titulo:    { color: '#cdd6f4', marginBottom: 24 },
  grid:      { display: 'grid', gridTemplateColumns: '340px 1fr', gap: 24, alignItems: 'start' },
};