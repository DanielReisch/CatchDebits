import apiClient from '../api/apiClient';
import type { Pessoa, PessoaRequest } from '../types';

export const getPessoas = () =>
  apiClient.get<Pessoa[]>('/Pessoas').then(res => res.data);

export const createPessoa = (data: PessoaRequest) =>
  apiClient.post<Pessoa>('/Pessoas', data).then(res => res.data);

export const deletePessoa = (id: number) =>
  apiClient.delete(`/Pessoas/${id}`);