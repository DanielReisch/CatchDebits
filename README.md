#  CatchDebits — Gerenciamento Financeiro por Pessoa

Aplicação full-stack desenvolvida como desafio técnico, para gerenciamento de transações financeiras vinculadas a pessoas, com regras de negócio, validações e relatório de totais.

---

## Requisitos do Desafio Implementados

### 1. Cadastro de Pessoas
- **Criar** nova pessoa (Nome + Idade)
- **Listar** todas as pessoas cadastradas
- **Deletar** pessoa pelo Id
- **Cascade Delete**: ao deletar uma pessoa, **todas as suas transações são removidas automaticamente** (configurado via `DeleteBehavior.Cascade` no Entity Framework Core)

### 2. Cadastro de Transações
- **Criar** transação vinculada a uma pessoa (Descrição, Valor, Tipo, PessoaId)
- **Listar** todas as transações com o nome da pessoa vinculada
- **Regra de Negócio Crítica**: pessoas com **menos de 18 anos** só podem registrar transações do tipo **Despesa**. Tentativas de registrar **Receita** para menores retornam HTTP 400 com mensagem explicativa
  - Validação aplicada no **back-end** (Controller) — fonte da verdade
  - Validação aplicada no **front-end** (UX) — desabilita a opção e exibe aviso visual

### 3. Relatório de Totais
- Lista todas as pessoas exibindo:
  - Total de Receitas
  - Total de Despesas  
  - Saldo Líquido (Receitas − Despesas)
- Exibe o **Total Geral acumulado** de todas as pessoas no rodapé da tabela
- Pessoas sem transações aparecem com saldo zerado

### 4. Extras
- Código bem documentado com `<summary>` XML e comentários explicativos de lógica
- Arquitetura em camadas (Controllers → Services → Repository/DbContext)
- Princípios SOLID aplicados (SRP, DIP via interfaces)
- Padrão RESTful nos endpoints

---

## Arquitetura do Projeto

CatchDebits/
├── src/
│   └── CatchDebits.API/          → Back-end: ASP.NET Core Web API
│       ├── Controllers/           → Endpoints REST (orquestração)
│       ├── Services/              → Lógica de negócio
│       ├── Data/                  → DbContext + configuração EF Core
│       ├── Models/                → Entidades do banco de dados
│       ├── DTOs/                  → Contratos de entrada e saída da API
│       └── Validators/            → Validações com FluentValidation
└── frontend/                      → Front-end: React + TypeScript + Vite
└── src/
├── api/                   → Instância configurada do Axios
├── services/              → Funções que chamam os endpoints
├── types/                 → Interfaces TypeScript (espelham os DTOs)
├── components/            → Componentes reutilizáveis (Form, List)
└── pages/                 → Páginas da aplicação

---

## Tecnologias Utilizadas

| Camada | Tecnologia |
|---|---|
| Back-end | .NET 10, C#, ASP.NET Core Web API |
| Banco de dados | SQLite + Entity Framework Core 10 |
| Validação | FluentValidation 11 |
| Documentação API | Swagger / OpenAPI 3.0 |
| Front-end | React 18, TypeScript, Vite 8 |
| HTTP Client | Axios |
| Roteamento | React Router DOM v6 |

---

## Como Executar o Projeto

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) ou superior
- [Node.js 18+](https://nodejs.org/) e npm

### 1. Clonar o repositório

```bash
git clone https://github.com/DanielReisch/CatchDebits.git
cd CatchDebits
```

### 2. Executar o Back-end

```bash
cd src/CatchDebits.API
dotnet run
```

- API disponível em: `http://localhost:5057`
- Swagger UI em: `http://localhost:5057/swagger`
- O banco de dados SQLite (`catchdebits.db`) é **criado automaticamente** na primeira execução

### 3. Executar o Front-end

Em um novo terminal:

```bash
cd frontend
npm install
npm run dev
```

- Aplicação disponível em: `http://localhost:5173`

---

## Endpoints da API

### Pessoas

| Método | Rota | Descrição | Status |
|---|---|---|---|
| `GET` | `/api/Pessoas` | Lista todas as pessoas | 200 |
| `POST` | `/api/Pessoas` | Cadastra nova pessoa | 201 |
| `DELETE` | `/api/Pessoas/{id}` | Remove pessoa e suas transações | 204 / 404 |

**Exemplo POST /api/Pessoas:**
```json
{
  "nome": "João Silva",
  "idade": 25
}
```

### Transações

| Método | Rota | Descrição | Status |
|---|---|---|---|
| `GET` | `/api/Transacoes` | Lista todas as transações | 200 |
| `POST` | `/api/Transacoes` | Cadastra nova transação | 201 / 400 / 404 |
| `GET` | `/api/Transacoes/totais` | Relatório de totais por pessoa | 200 |

**Exemplo POST /api/Transacoes:**
```json
{
  "descricao": "Salário",
  "valor": 3000.00,
  "tipo": 1,
  "pessoaId": 1
}
```
> `tipo`: `0` = Despesa | `1` = Receita

---

## Regras de Negócio

### Cascade Delete
Ao deletar uma pessoa via `DELETE /api/Pessoas/{id}`, todas as transações vinculadas são removidas automaticamente pelo banco de dados, sem necessidade de lógica adicional no código C#.

```csharp
// Configurado no AppDbContext via Fluent API
entity.HasOne(t => t.Pessoa)
      .WithMany(p => p.Transacoes)
      .HasForeignKey(t => t.PessoaId)
      .OnDelete(DeleteBehavior.Cascade); // ← apaga transações junto
```

### Restrição de Idade para Receita
Pessoas com menos de 18 anos **não podem** registrar transações do tipo **Receita**.

```csharp
// Validado no TransacoesController antes de persistir
if (pessoa.Idade < 18 && dto.Tipo == TipoTransacao.Receita)
    return BadRequest(new { 
        mensagem = "Menores de 18 anos não podem registrar Receitas." 
    });
```

No front-end, a opção "Receita" é **desabilitada automaticamente** ao selecionar um menor de idade, com aviso visual — a validação do back-end permanece como fonte da verdade.

---

## Decisões de Arquitetura

| Decisão | Justificativa |
|---|---|
| **SQLite** | Zero configuração de servidor, arquivo único `.db`, ideal para avaliação e deploy |
| **DTOs separados** | Desacoplam o contrato da API das entidades do banco; permitem evolução independente |
| **Interfaces nos Services** | Aplicam o Dependency Inversion Principle (D do SOLID); facilitam testes e substituição |
| **AsNoTracking()** | Usado em todas as queries de leitura para melhor performance (sem overhead de rastreamento) |
| **EnsureCreated()** | Cria o banco e as tabelas automaticamente na primeira execução, sem necessidade de migrations manuais |
| **Validação em dois níveis** | Back-end (fonte da verdade) + Front-end (UX) — segurança sem sacrificar usabilidade |

---

## Estrutura do Banco de Dados

Tabela: Pessoas
├── Id       INTEGER PRIMARY KEY AUTOINCREMENT
├── Nome     TEXT NOT NULL (max 150)
└── Idade    INTEGER NOT NULL
Tabela: Transacoes
├── Id        INTEGER PRIMARY KEY AUTOINCREMENT
├── Descricao TEXT NOT NULL (max 255)
├── Valor     DECIMAL(18,2) NOT NULL
├── Tipo      INTEGER NOT NULL (0=Despesa, 1=Receita)
└── PessoaId  INTEGER NOT NULL → FK Pessoas(Id) ON DELETE CASCADE

---

## Uso de Inteligência Artificial

Este projeto foi desenvolvido com suporte estratégico de **IA generativa (Claude - Anthropic)**, como ferramenta de apoio técnico.

### Como a IA foi utilizada

| Área | Uso |
|---|---|
| **Arquitetura** | Discussão e validação das decisões de design (camadas, padrões, SOLID) |
| **Resolução de erros** | Diagnóstico de erros de compilação e incompatibilidades de versão (.NET 10, FluentValidation) |
| **Documentação** | Estruturação dos comentários XML e do próprio README |

### O que foi feito pelo desenvolvedor

- Todas as decisões técnicas finais foram avaliadas e aprovadas
- Cada trecho de código foi lido, compreendido e adaptado ao contexto do projeto
- A implementação foi executada manualmente, arquivo por arquivo
- Bugs e incompatibilidades foram investigados e resolvidos ativamente
