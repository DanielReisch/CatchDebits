export interface Pessoa {
  id: number;
  nome: string;
  idade: number;
}

export interface PessoaRequest {
  nome: string;
  idade: number;
}

// Substitui enum por const object (compatível com erasableSyntaxOnly)
export const TipoTransacao = {
  Despesa: 0,
  Receita: 1,
} as const;

export type TipoTransacao = typeof TipoTransacao[keyof typeof TipoTransacao];

export interface Transacao {
  id: number;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  tipoDescricao: string;
  pessoaId: number;
  nomePessoa: string;
}

export interface TransacaoRequest {
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  pessoaId: number;
}

export interface TotaisPessoa {
  pessoaId: number;
  nomePessoa: string;
  idadePessoa: number;
  totalReceitas: number;
  totalDespesas: number;
  saldoLiquido: number;
}

export interface RelatorioTotais {
  totaisPorPessoa: TotaisPessoa[];
  totalReceitasGeral: number;
  totalDespesasGeral: number;
  saldoLiquidoGeral: number;
}