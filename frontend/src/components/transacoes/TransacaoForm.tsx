import { useState, useEffect } from 'react';
import { TipoTransacao } from '../../types';
import type { Pessoa, TransacaoRequest } from '../../types';
import { getPessoas } from '../../services/pessoaService';
import { createTransacao } from '../../services/transacaoService';

interface Props {
  onSuccess: () => void;
}

export function TransacaoForm({ onSuccess }: Props) {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [pessoaSelecionada, setPessoaSelecionada] = useState<Pessoa | null>(null);
  const [form, setForm] = useState<TransacaoRequest>({ descricao: '', valor: 0, tipo: TipoTransacao.Despesa, pessoaId: 0 });
  const [erro, setErro] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => { getPessoas().then(setPessoas); }, []);

  const handlePessoaChange = (id: number) => {
    const pessoa = pessoas.find(p => p.id === id) ?? null;
    setPessoaSelecionada(pessoa);
    // Se menor de 18, força tipo Despesa para refletir a regra de negócio
    const tipo = pessoa && pessoa.idade < 18 ? TipoTransacao.Despesa : form.tipo;
    setForm(prev => ({ ...prev, pessoaId: id, tipo }));
    setErro('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErro('');

    // Validação client-side (a real está no back-end, esta é só para UX)
    if (pessoaSelecionada && pessoaSelecionada.idade < 18 && form.tipo === TipoTransacao.Receita) {
      setErro('Menores de 18 anos não podem registrar Receitas.');
      return;
    }

    try {
      setLoading(true);
      await createTransacao(form);
      setForm(prev => ({ ...prev, descricao: '', valor: 0 }));
      onSuccess();
    } catch (err: any) {
      const erros = err.response?.data?.errors;
      if (erros) {
        setErro(Object.values(erros).flat().join(' '));
      } else {
        setErro(err.response?.data?.title ?? 'Erro ao criar transação.');
      }
    } finally {
      setLoading(false);
    }
  };

  const eMenor = pessoaSelecionada !== null && pessoaSelecionada.idade < 18;

  return (
    <form onSubmit={handleSubmit} style={styles.form}>
      <h3 style={styles.title}>Nova Transação</h3>

      <select style={styles.input} value={form.pessoaId || ''} onChange={e => handlePessoaChange(Number(e.target.value))} required>
        <option value="" disabled>Selecione uma pessoa...</option>
        {pessoas.map(p => (
          <option key={p.id} value={p.id}>
            {p.nome} ({p.idade} anos){p.idade < 18 ? ' ⚠️ Menor de idade' : ''}
          </option>
        ))}
      </select>

      <input style={styles.input} placeholder="Descrição" value={form.descricao}
        onChange={e => setForm(prev => ({ ...prev, descricao: e.target.value }))} required />

      <input style={styles.input} type="number" step="0.01" min="0.01" placeholder="Valor (R$)"
        value={form.valor || ''}
        onChange={e => setForm(prev => ({ ...prev, valor: parseFloat(e.target.value) }))} required />

      <select style={styles.input} value={form.tipo} onChange={e => setForm(prev => ({ ...prev, tipo: Number(e.target.value) as TipoTransacao }))}>
        <option value={TipoTransacao.Despesa}>Despesa</option>
        <option value={TipoTransacao.Receita} disabled={eMenor}>
          Receita {eMenor ? '(não permitido para menores)' : ''}
        </option>
      </select>

      {eMenor && <p style={styles.aviso}>⚠️ Menor de 18 anos — apenas Despesas são permitidas.</p>}
      {erro   && <p style={styles.erro}>{erro}</p>}

      <button style={styles.button} type="submit" disabled={loading || !form.pessoaId}>
        {loading ? 'Salvando...' : '+ Adicionar Transação'}
      </button>
    </form>
  );
}

const styles: Record<string, React.CSSProperties> = {
  form:  { display: 'flex', flexDirection: 'column', gap: 10, padding: 20, background: '#1e1e2e', borderRadius: 8 },
  title: { margin: 0, color: '#cdd6f4', fontSize: 16 },
  input: { padding: '8px 12px', borderRadius: 6, border: '1px solid #45475a', background: '#313244', color: '#cdd6f4', fontSize: 14 },
  button:{ padding: 10, borderRadius: 6, border: 'none', background: '#a6e3a1', color: '#1e1e2e', fontWeight: 'bold', cursor: 'pointer' },
  erro:  { color: '#f38ba8', margin: 0, fontSize: 13 },
  aviso: { color: '#fab387', margin: 0, fontSize: 13 },
};