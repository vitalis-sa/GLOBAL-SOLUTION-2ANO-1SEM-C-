# HyDrata – API de Gestão (.NET)

> Módulo administrativo da plataforma **HyDrata** — sistema de monitoramento hídrico e otimização de irrigação para pequenos e médios produtores rurais.

O HyDrata cruza dados de satélites (ANA, INPE) com sensores ESP32 instalados no campo para responder uma pergunta simples ao produtor: **IRRIGAR HOJE? SIM ou NÃO.** Esta API é responsável pelo cadastro e gestão de produtores, cooperativas, planos de assinatura e propriedades rurais — o módulo administrativo da plataforma.

---

## Contexto da Solução Completa

O HyDrata é composto por múltiplas disciplinas que trabalham em conjunto:

| Camada | Tecnologia | Responsabilidade |
|---|---|---|
| **IoT** | ESP32 + MQTT | Sensor de umidade do solo e luminosidade no campo |
| **API Core** | Java Spring Boot | Consome ANA/INPE/Open-Meteo, processa regras de irrigação, gera alertas |
| **API Gestão** | **.NET 9 (este repositório)** | CRUD de produtores, cooperativas, planos e propriedades |
| **Banco de Dados** | Oracle + PL/SQL | Procedures, triggers e packages com regras de negócio |
| **Mobile** | React Native | 5 telas — dashboard, alertas, histórico, mapa e cadastro |
| **DevOps** | Docker + Azure | Containers da API Java e Oracle em nuvem |

> Esta API `.NET` **não interage** com as tabelas de monitoramento (`ALERTA`, `LEITURA_CLIMA`, `LEITURA_LUZ`, `DISPOSITIVO_IOT`). Essas tabelas são responsabilidade exclusiva da API Java. O `.NET` gerencia apenas as **5 tabelas de gestão** descritas abaixo.

---

## Tecnologias

| Tecnologia | Versão |
|---|---|
| .NET / ASP.NET Core | 9.0 |
| Entity Framework Core | 9.0.5 |
| Oracle.EntityFrameworkCore | 9.23.60 |
| Swashbuckle (Swagger) | 7.3.1 |

---

## Arquitetura

O projeto adota arquitetura em **3 camadas**:

```
┌──────────────────────────────────────────────────────┐
│                   Controllers                        │  ← Recebe HTTP, valida, retorna respostas
├──────────────────────────────────────────────────────┤
│                  Repositories                        │  ← Abstração do acesso a dados (interfaces + impl.)
├──────────────────────────────────────────────────────┤
│              Dados (AppDbContext / EF Core)          │  ← Mapeamento ORM → Oracle
└──────────────────────────────────────────────────────┘
```

### Estrutura de Pastas

```
GLOBAL-SOLUTION-2ANO-1SEM-C#/
├── Controllers/
│   ├── ProdutoresApiController.cs
│   ├── CooperativasApiController.cs
│   ├── PlanosApiController.cs
│   ├── PropriedadesApiController.cs
│   └── ProdutorCooperativaApiController.cs
├── Repositories/
│   ├── IRepositories.cs              ← Interfaces de todos os repositórios
│   ├── ProdutorRepository.cs
│   ├── CooperativaRepository.cs
│   ├── PlanoRepository.cs
│   ├── PropriedadeRepository.cs
│   └── ProdutorCooperativaRepository.cs
├── Dto/
│   ├── ProdutorDtos.cs
│   ├── CooperativaDtos.cs
│   ├── PlanoDtos.cs
│   ├── PropriedadeDtos.cs
│   └── ProdutorCooperativaDtos.cs
├── Models/
│   ├── Produtor.cs
│   ├── Cooperativa.cs
│   ├── Plano.cs
│   ├── Propriedade.cs
│   └── ProdutorCooperativa.cs
├── Dados/
│   └── AppDbContext.cs
├── Migrations/                       (gerado pelo EF Core)
├── Dockerfile
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

---

## Modelagem do Banco

### Diagrama de Entidades (tabelas gerenciadas por esta API)

> 

```
┌──────────────┐        1:N        ┌──────────────────┐
│    PLANO     │─────────────────►│   PROPRIEDADE     │
└──────────────┘                   └──────────────────┘
                                           ▲  N:1
                                           │
┌──────────────┐        1:N        ┌───────┴──────────┐
│   PRODUTOR   │─────────────────►│   PROPRIEDADE     │
│              │                   └──────────────────┘
│              │  N:N (via junção) ┌──────────────────────────┐
│              │─────────────────►│   PRODUTOR_COOPERATIVA   │◄──┐
└──────────────┘                   │   ProdutorId (PK, FK)    │   │
                                   │   CooperativaId (PK, FK) │   │
                                   └──────────────────────────┘   │
                                                     ┌────────────┴──┐
                                                     │  COOPERATIVA  │
                                                     └───────────────┘
```

### Tabelas gerenciadas por esta API

| Tabela Oracle | Descrição |
|---|---|
| `PRODUTOR` | Produtores rurais cadastrados no sistema |
| `COOPERATIVA` | Cooperativas agrícolas parceiras |
| `PLANO` | Planos de assinatura (Básico R$49, Pro R$99, Cooperativa R$29) |
| `PROPRIEDADE` | Propriedades rurais monitoradas, vinculadas a produtor e plano |
| `PRODUTOR_COOPERATIVA` | Tabela de junção para o N:N Produtor ↔ Cooperativa |

### Tabelas do banco NÃO gerenciadas por esta API

As tabelas abaixo existem no mesmo schema Oracle, mas são manipuladas exclusivamente pela **API Java**:

| Tabela Oracle | Responsável | Descrição |
|---|---|---|
| `DISPOSITIVO_IOT` | Java | Sensores ESP32 instalados nas propriedades |
| `LEITURA_CLIMA` | Java | Leituras de umidade do solo enviadas via MQTT |
| `LEITURA_LUZ` | Java | Leituras de luminosidade enviadas via MQTT |
| `ALERTA` | Java | Alertas gerados pelo motor de regras de irrigação |

---

## Relacionamentos

### 1:N — Produtor → Propriedades

Um produtor pode ter várias propriedades. Cada propriedade pertence a exatamente um produtor. Configurado com `OnDelete(Restrict)` — a exclusão de um produtor que possua propriedades vinculadas é bloqueada.

> ⚠️ O `Delete` do `ProdutorRepository` executa um bloco PL/SQL que remove em cascata os registros dependentes nas tabelas Java (`ALERTA`, `LEITURA_CLIMA`, `LEITURA_LUZ`, `DISPOSITIVO_IOT`, `PROPRIEDADE`) antes de remover o produtor, garantindo integridade mesmo sem cascade configurado no EF.

### 1:N — Plano → Propriedades

Um plano pode ser usado por várias propriedades. Configurado com `OnDelete(Restrict)` — não é possível deletar um plano em uso.

### N:N — Produtor ↔ Cooperativa

Um produtor pode pertencer a várias cooperativas, e vice-versa. Implementado via tabela de junção `PRODUTOR_COOPERATIVA` com **chave primária composta** `(ProdutorId, CooperativaId)`. Configurado com `OnDelete(Cascade)` — ao deletar um produtor ou cooperativa, as associações são removidas automaticamente.

---

## Como Executar

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Acesso ao Oracle (FIAP ou instância própria)

### 1. Clonar o repositório

```bash
git clone https://github.com/vitalis-sa/GLOBAL-SOLUTION-2ANO-1SEM-C-
cd GLOBAL-SOLUTION-2ANO-1SEM-C-
```

### 2. Configurar a connection string

O projeto já vem configurado com a connection string do Oracle da FIAP em `appsettings.json`. Para usar outro banco, edite:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "Data Source=oracle.fiap.com.br:1521/orcl;User Id=SEU_RM;Password=SUA_SENHA;"
  }
}
```

### 3. Executar a API

```bash
dotnet run
```

Acesse o Swagger em: **http://localhost:5000/swagger**

> As tabelas já existem no Oracle da FIAP (criadas pelo time de Banco de Dados). Não é necessário rodar migrations manualmente — o schema é compartilhado com a API Java.

---

## Migrations

As migrations estão incluídas no repositório (`Migrations/`). Caso precise recriar o schema em um banco próprio:

```bash
# Instalar o EF CLI (uma vez, global)
dotnet tool install --global dotnet-ef

# Aplicar as migrations no banco configurado no appsettings.json
dotnet ef database update

# Criar nova migration após alterar um Model
dotnet ef migrations add NomeDaMudanca

# Reverter para uma migration anterior
dotnet ef database update NomeDaMigrationAnterior

# Remover a última migration (se ainda não foi aplicada)
dotnet ef migrations remove

# Listar status das migrations
dotnet ef migrations list
```

---

## Endpoints

### Produtores — `/api/produtores`

| Método | Rota | Status de sucesso | Descrição |
|---|---|---|---|
| `GET` | `/api/produtores` | 200 | Lista todos os produtores |
| `GET` | `/api/produtores/{id}` | 200 | Busca produtor por ID |
| `GET` | `/api/produtores/{id}/propriedades` | 200 | Produtor com suas propriedades e planos vinculados |
| `GET` | `/api/produtores/email/{email}` | 200 | Busca produtor por e-mail |
| `POST` | `/api/produtores` | 201 | Cadastra novo produtor |
| `PUT` | `/api/produtores/{id}` | 200 | Atualiza dados do produtor |
| `DELETE` | `/api/produtores/{id}` | 204 | Remove produtor e todos os seus dados dependentes |

### Cooperativas — `/api/cooperativas`

| Método | Rota | Status de sucesso | Descrição |
|---|---|---|---|
| `GET` | `/api/cooperativas` | 200 | Lista todas |
| `GET` | `/api/cooperativas/{id}` | 200 | Busca por ID |
| `POST` | `/api/cooperativas` | 201 | Cadastra nova |
| `PUT` | `/api/cooperativas/{id}` | 200 | Atualiza |
| `DELETE` | `/api/cooperativas/{id}` | 204 | Remove (associações são removidas em cascata) |

### Planos — `/api/planos`

| Método | Rota | Status de sucesso | Descrição |
|---|---|---|---|
| `GET` | `/api/planos` | 200 | Lista todos |
| `GET` | `/api/planos/{id}` | 200 | Busca por ID |
| `POST` | `/api/planos` | 201 | Cadastra novo |
| `PUT` | `/api/planos/{id}` | 200 | Atualiza |
| `DELETE` | `/api/planos/{id}` | 204 | Remove (falha se houver propriedades usando o plano) |

### Propriedades — `/api/propriedades`

| Método | Rota | Status de sucesso | Descrição |
|---|---|---|---|
| `GET` | `/api/propriedades` | 200 | Lista todas (aceita `?produtorId=1` para filtrar) |
| `GET` | `/api/propriedades/{id}` | 200 | Busca por ID (retorna nome do produtor e do plano) |
| `POST` | `/api/propriedades` | 201 | Cadastra nova (valida se produtor e plano existem) |
| `PUT` | `/api/propriedades/{id}` | 200 | Atualiza |
| `DELETE` | `/api/propriedades/{id}` | 204 | Remove |

### Associações Produtor-Cooperativa — `/api/produtorcooperativa`

| Método | Rota | Status de sucesso | Descrição |
|---|---|---|---|
| `GET` | `/api/produtorcooperativa` | 200 | Lista todas as associações |
| `GET` | `/api/produtorcooperativa/produtor/{produtorId}` | 200 | Cooperativas de um produtor |
| `GET` | `/api/produtorcooperativa/cooperativa/{cooperativaId}` | 200 | Produtores de uma cooperativa |
| `POST` | `/api/produtorcooperativa` | 201 | Associa produtor a cooperativa |
| `DELETE` | `/api/produtorcooperativa/{produtorId}/{cooperativaId}` | 204 | Remove associação |

---

## Exemplos de Uso

### Criar um Plano

```bash
curl -X POST http://localhost:5000/api/planos \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Pro",
    "valorMensalidade": 99.00,
    "descricao": "Plano Básico + kit sensor ESP32 + suporte",
    "status": "ATIVO"
  }'
```

Resposta `201 Created`:
```json
{
  "id": 1,
  "nome": "Pro",
  "valorMensalidade": 99.00,
  "descricao": "Plano Básico + kit sensor ESP32 + suporte",
  "status": "ATIVO"
}
```

### Criar um Produtor

```bash
curl -X POST http://localhost:5000/api/produtores \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João da Silva",
    "cpf": "123.456.789-00",
    "email": "joao@fazenda.com",
    "telefone": "(11) 98765-4321",
    "senha": "senha123",
    "status": "ATIVO"
  }'
```

CPF ou e-mail duplicado retorna `409 Conflict`:
```json
{ "erro": "CPF '123.456.789-00' já está cadastrado." }
```

### Criar uma Cooperativa

```bash
curl -X POST http://localhost:5000/api/cooperativas \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "CoopAgro SP",
    "email": "contato@coopagrosp.com.br",
    "telefone": "(11) 3000-0000",
    "status": "ATIVA"
  }'
```

### Criar uma Propriedade

```bash
curl -X POST http://localhost:5000/api/propriedades \
  -H "Content-Type: application/json" \
  -d '{
    "produtorId": 1,
    "planoId": 1,
    "nome": "Fazenda Boa Vista",
    "areaHectares": 250.5,
    "cidade": "Ribeirão Preto",
    "estado": "SP",
    "latitude": -21.1775,
    "longitude": -47.8106,
    "status": "ATIVA"
  }'
```

Produtor ou plano inexistente retorna `404 Not Found`:
```json
{ "erro": "Produtor com ID 99 não encontrado." }
```

### Associar Produtor a Cooperativa

```bash
curl -X POST http://localhost:5000/api/produtorcooperativa \
  -H "Content-Type: application/json" \
  -d '{
    "produtorId": 1,
    "cooperativaId": 1
  }'
```

Associação duplicada retorna `409 Conflict`:
```json
{ "erro": "Esse produtor já está associado a essa cooperativa." }
```

### Listar propriedades de um produtor

```bash
curl "http://localhost:5000/api/propriedades?produtorId=1"
```

### Buscar produtor com propriedades e planos

```bash
curl http://localhost:5000/api/produtores/1/propriedades
```

Resposta inclui o plano de cada propriedade:
```json
{
  "id": 1,
  "nome": "João da Silva",
  "propriedades": [
    {
      "id": 1,
      "nome": "Fazenda Boa Vista",
      "areaHectares": 250.5,
      "status": "ATIVA",
      "plano": { "id": 1, "nome": "Pro" }
    }
  ]
}
```

### Remover Associação N:N

```bash
curl -X DELETE http://localhost:5000/api/produtorcooperativa/1/1
# → 204 No Content
```

---

## Comportamento ao Deletar

| Entidade deletada | O que acontece |
|---|---|
| **Produtor** | Remove em cascata via PL/SQL: alertas, leituras (clima e luz), dispositivos IoT, propriedades e associações com cooperativas são deletados antes do produtor. |
| **Cooperativa** | Associações com produtores são removidas automaticamente (`Cascade` no EF). |
| **Plano** | ❌ Bloqueado com `409 Conflict` se houver propriedades usando o plano (`ORA-02292`). |
| **Propriedade** | Removida diretamente, sem impacto nas outras tabelas de gestão. |
| **Associação Produtor-Cooperativa** | Removida diretamente — produtor e cooperativa permanecem intactos. |

---

## Validações de Negócio

| Regra | Resposta |
|---|---|
| CPF duplicado ao cadastrar produtor | `409 Conflict` |
| E-mail duplicado ao cadastrar/atualizar produtor | `409 Conflict` |
| `ProdutorId` inexistente ao criar propriedade | `404 Not Found` |
| `PlanoId` inexistente ao criar/atualizar propriedade | `404 Not Found` |
| `ProdutorId` ou `CooperativaId` inexistente ao associar | `404 Not Found` |
| Associação Produtor-Cooperativa duplicada | `409 Conflict` |
| Plano em uso ao tentar deletar | `409 Conflict` |

---

## Docker

O projeto inclui `Dockerfile` com build multistage (build em SDK → runtime em Alpine), usuário não-root e porta 5000 exposta:

```bash
# Build da imagem
docker build -t hydrata-gestao-api .

# Executar o container
docker run -d \
  --name hydrata-gestao \
  -p 5000:5000 \
  -e ConnectionStrings__OracleConnection="Data Source=oracle.fiap.com.br:1521/orcl;User Id=SEU_RM;Password=SUA_SENHA;" \
  hydrata-gestao-api
```

Acesse o Swagger em: **http://localhost:5000/swagger**

---

## Integrantes

**Turma:** 2TDSpV

| Nome | RM |
|---|---|
| Ana Flavia Camelo | RM561489 |
| Gustavo Kenji Terada | RM562745 |
| João Guilherme Carvalho Novaes | RM566234 |
| Pedro Chasci Puga | RM565154 |
| Lucas Figueiredo Vieira | RM561342 |

---

## Modelagem de Dados Completa

<img width="995" height="659" alt="WhatsApp Image 2026-06-08 at 22 36 45" src="https://github.com/user-attachments/assets/74c1fc53-91f0-411f-80cc-78523d08f9a8" />

<img width="995" height="659" alt="WhatsApp Image 2026-06-08 at 22 36 45 (1)" src="https://github.com/user-attachments/assets/2e3279dd-8b98-4929-b54d-d7c02d1c63a2" />


*HyDrata — Global Solution 2026/1 — FIAP — Análise e Desenvolvimento de Sistemas*
