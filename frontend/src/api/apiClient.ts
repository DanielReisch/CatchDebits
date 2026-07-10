import axios from 'axios';

/**
 * Instância centralizada do Axios.
 * Toda chamada à API usa esta instância, garantindo:
 * - BaseURL configurada em um único lugar
 * - Headers padrão (Content-Type)
 */
const apiClient = axios.create({
  baseURL: 'http://localhost:5057/api',
  headers: { 'Content-Type': 'application/json' },
});

export default apiClient;