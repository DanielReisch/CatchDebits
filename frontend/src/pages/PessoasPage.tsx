import { useState, useEffect, useCallback } from 'react';
import type { Pessoa } from '../types';
import { getPessoas, deletePessoa } from '../services/pessoaService';
import { PessoaForm } from '../components/pessoas/PessoaForm';
import { PessoaList } from '../components/pessoas/PessoaList';

export function PessoasPage() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);

  const carregar = useCallback(async () => {
    const data = await getPessoas();
    setPessoas(data);
  }, []);

  useEffect(() => { carregar(); }, [carregar]);

  const handleDeletar = async (id: number) => {
    if (!confirm('Deletar esta pessoa e todas as suas transações?')) return;
    await deletePessoa(id);
    carregar();
  };

  return (
    <div style={styles.container}>
      <h2 style={styles.titulo}>👤 Pessoas</h2>
      <div style={styles.grid}>
        <PessoaForm onSuccess={carregar} />
        <PessoaList pessoas={pessoas} onDeletar={handleDeletar} />
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  container: { maxWidth: 900, margin: '0 auto' },
  titulo:    { color: '#cdd6f4', marginBottom: 24 },
  grid:      { display: 'grid', gridTemplateColumns: '320px 1fr', gap: 24, alignItems: 'start' },
};