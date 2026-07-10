import { BrowserRouter, Link, Route, Routes, useLocation } from 'react-router-dom';
import { PessoasPage } from './pages/PessoasPage';
import { TransacoesPage } from './pages/TransacoesPage';
import { TotaisPage } from './pages/TotaisPage';

function Nav() {
  const { pathname } = useLocation();
  const link = (to: string, label: string) => (
    <Link to={to} style={{ ...styles.link, ...(pathname === to ? styles.linkAtivo : {}) }}>
      {label}
    </Link>
  );
  return (
    <nav style={styles.nav}>
      <span style={styles.logo}>💰 CatchDebits</span>
      <div style={styles.links}>
        {link('/pessoas', '👤 Pessoas')}
        {link('/transacoes', '💸 Transações')}
        {link('/totais', '📊 Totais')}
      </div>
    </nav>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <div style={styles.app}>
        <Nav />
        <main style={styles.main}>
          <Routes>
            <Route path="/"           element={<PessoasPage />} />
            <Route path="/pessoas"    element={<PessoasPage />} />
            <Route path="/transacoes" element={<TransacoesPage />} />
            <Route path="/totais"     element={<TotaisPage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

const styles: Record<string, React.CSSProperties> = {
  app:      { minHeight: '100vh', background: '#181825', fontFamily: 'sans-serif' },
  nav:      { display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '16px 32px', background: '#1e1e2e', borderBottom: '1px solid #313244' },
  logo:     { color: '#89b4fa', fontWeight: 'bold', fontSize: 20 },
  links:    { display: 'flex', gap: 24 },
  link:     { color: '#6c7086', textDecoration: 'none', fontSize: 15, padding: '6px 12px', borderRadius: 6 },
  linkAtivo:{ color: '#cdd6f4', background: '#313244' },
  main:     { padding: '32px' },
};