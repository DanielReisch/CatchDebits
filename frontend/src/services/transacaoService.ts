import apiClient from '../api/apiClient';
import type { RelatorioTotais, Transacao, TransacaoRequest } from '../types';

export const getTransacoes = () =>
  apiClient.get<Transacao[]>('/Transacoes').then(res => res.data);

export const createTransacao = (data: TransacaoRequest) =>
  apiClient.post<Transacao>('/Transacoes', data).then(res => res.data);

export const getRelatorioTotais = () =>
  apiClient.get<RelatorioTotais>('/Transacoes/totais').then(res => res.data);