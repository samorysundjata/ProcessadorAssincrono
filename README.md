# Processador Assíncrono

Este repositório implementa uma aplicação .NET 10 baseada em **Clean Architecture**, com foco em **processamento assíncrono em lote** utilizando `BackgroundService` e `Channel<Aprovacao>`, com persistência no **SQL Server** via **Dapper**.

---

### Badges

![GitHub repo size](https://img.shields.io/github/repo-size/samoryfiotec/Fiotec.ProcessadorAssincrono?label=RepoSize&color=brown&style=flat&suffix=KB)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Dapper](https://img.shields.io/badge/Dapper-Library-007ACC?style=flat-square)](https://github.com/DapperLib/Dapper)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Server-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![BackgroundService](https://img.shields.io/badge/BackgroundService-Hosted-0078D4?style=flat-square)](https://learn.microsoft.com/dotnet/core/extensions/background-services)
[![Channel<Aprovacao>](https://img.shields.io/badge/Channel-%3CAprovacao%3E-00ABA9?style=flat-square)](https://learn.microsoft.com/dotnet/standard/parallel-programming/channels)
[![Minimal APIs](https://img.shields.io/badge/Minimal_APIs-.NET-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
[![xUnit](https://img.shields.io/badge/xUnit-Tests-512BD4?style=flat-square&logo=xunit&logoColor=white)](https://xunit.net/)
[![Moq](https://img.shields.io/badge/Moq-Mocking-9B4F96?style=flat-square)](https://github.com/moq)
[![Shouldly](https://img.shields.io/badge/Shouldly-Assertions-8A2BE2?style=flat-square)](https://shouldly.readthedocs.io/en/latest/)
![Build Status](https://github.com/samorysundjata/ProcessadorAssincrono/actions/workflows/dotnet.yml/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](./LICENSE)

---

## Estrutura do Projeto

```text
ProcessadorAssincrono/
├── ProcessadorAssincrono.slnx
├── global.json                          → SDK 10.0.112
├── docker-compose.yml                   → SQL Server, criação do banco e API
├── docker/init-db.sql                   → banco Processador e tabela Aprovacoes
├── src/ProcessadorAssincrono.API        → Minimal APIs
├── src/ProcessadorAssincrono.Application
├── src/ProcessadorAssincrono.Domain
├── src/ProcessadorAssincrono.Infrastructure
└── tests/ProcessadorAssincrono.Tests    → xUnit, Shouldly e Moq
```

---

## Tecnologias Utilizadas

- .NET 10
- Dapper
- SQL Server
- BackgroundService
- `Channel<Guid>`
- Minimal APIs
- Polly

---

## Componentes Principais

### `BackgroundService` com `Channel<Aprovacao>`

Enfileira solicitações para processamento em segundo plano, desacoplando a chamada HTTP da persistência.

### `AprovacaoService` com Dapper

Realiza a atualização da entidade `Aprovacao` no banco SQL Server, marcando como aprovada.

### Minimal API

| Método | Rota | Efeito |
|---|---|---|
| `PUT` | `/api/solicitacoes/{id}/inserir` | Grava a solicitação no SQL Server |
| `PUT` | `/api/solicitacoes/{id}/aprovar` | Enfileira uma solicitação |
| `POST` | `/aprovar-em-lote` | Enfileira várias solicitações |

---

## Como executar

O `global.json` fixa o SDK **10.0.112** com `rollForward: latestPatch`. Um SDK `10.0.4xx` instalado na máquina não atende esse pin.

### Docker Compose

Sobe o SQL Server, cria o banco `Processador` e a tabela `Aprovacoes`, e inicia a API.

```bash
docker compose up --build -d
```

- API e Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)
- SQL Server: `localhost,1433`, usuário `sa`, senha `SenhaForte123!`

```bash
docker compose down
```

O volume `sqlserver-data` preserva o banco entre as execuções. O script `docker/init-db.sql` só cria o banco e a tabela quando eles ainda não existem.

### Na máquina local

Com o SQL Server já acessível em `localhost,1433` (por exemplo, `docker compose up -d sqlserver db-init`):

```powershell
$env:PATH = "$env:LOCALAPPDATA\Microsoft\dotnet;" + $env:PATH
dotnet run --project src/ProcessadorAssincrono.API --launch-profile http
```

Swagger local: [http://localhost:5085/swagger](http://localhost:5085/swagger).

A connection string padrão está em `src/ProcessadorAssincrono.API/appsettings.json`. No Compose, a API usa o host `sqlserver` em vez de `localhost`.

### Testes

```bash
dotnet test ProcessadorAssincrono.slnx
```

## Collection

A collection do Insomnia está em `docs/Files/ProcessadorAssincrono_Insomnia_2025-12-12.yaml`.

## Arquitetura

### Contexto

![Diagrama de Contexto](./out/docs/Context/Context.png)

### Sequência do BackgroundService

![Diagrama de Sequencia](./out/docs/C4/Sequence/ProcessadorQueueService%20Sequence.png)

## Banco de dados

O Compose aplica `docker/init-db.sql`. O schema resultante é:

```sql
CREATE TABLE Aprovacoes (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Projeto NVARCHAR(100) NOT NULL,
    ComentariosAdicionais NVARCHAR(MAX) NULL,
    DataAprovacao DATETIME2 NOT NULL
);
```

### Licença

Este projeto está licenciado sob a Licença [MIT](./LICENSE)
