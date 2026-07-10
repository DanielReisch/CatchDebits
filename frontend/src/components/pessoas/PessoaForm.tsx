import { useState } from 'react';
import type { PessoaRequest } from '../../types';
import { createPessoa } from '../../services/pessoaService';

interface Props {
  onSuccess: () => void;
}

export function PessoaForm({ onSuccess }: Props) {
  const [form, setForm] = useState<PessoaRequest>({ nome: '', idade: 0 });
  const [erro, setErro] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErro('');

    if (!form.nome.trim()) { setErro('Nome é obrigatório.'); return; }
    if (form.idade <= 0)    { setErro('Idade deve ser maior que zero.'); return; }

    try {
      setLoading(true);
      await createPessoa(form);
      setForm({ nome: '', idade: 0 });
      onSuccess();
    } catch {
      setErro('Erro ao cadastrar pessoa.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} style={styles.form}>
      <h3 style={styles.title}>Nova Pessoa</h3>

      <input
        style={styles.input}
        placeholder="Nome completo"
        value={form.nome}
        onChange={e => setForm(prev => ({ ...prev, nome: e.target.value }))}
      />

      <input
        style={styles.input}
        type="number"
        placeholder="Idade"
        min={1}
        value={form.idade || ''}
        onChange={e => setForm(prev => ({ ...prev, idade: Number(e.target.value) }))}
      />

      {erro && <p style={styles.erro}>{erro}</p>}

      <button style={styles.button} type="submit" disabled={loading}>
        {loading ? 'Salvando...' : '+ Adicionar Pessoa'}
      </button>
    </form>
  );
}

const styles: Record<string, React.CSSProperties> = {
  form:   { display: 'flex', flexDirection: 'column', gap: 10, padding: 20, background: '#1e1e2e', borderRadius: 8 },
  title:  { margin: 0, color: '#cdd6f4', fontSize: 16 },
  input:  { padding: '8px 12px', borderRadius: 6, border: '1px solid #45475a', background: '#313244', color: '#cdd6f4', fontSize: 14 },
  button: { padding: '10px', borderRadius: 6, border: 'none', background: '#89b4fa', color: '#1e1e2e', fontWeight: 'bold', cursor: 'pointer' },
  erro:   { color: '#f38ba8', margin: 0, fontSize: 13 },
};