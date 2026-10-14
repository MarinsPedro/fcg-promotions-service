# Fiap Cloud Games — Promotions Service

Serviço responsável por campanhas promocionais e cálculo de descontos da plataforma Fiap Cloud Games.

Este repositório é independente do monólito. Ele possui banco, migrations, pipeline e ciclo de entrega próprios.

## Responsabilidades

- cadastrar, consultar e desativar campanhas promocionais;
- descontos fixos ou percentuais;
- período de vigência e elegibilidade por jogo;
- cálculo do desconto sobre um preço-base recebido;
- retorno do preço final ao serviço consumidor.

Promotions **não** consulta o banco do Catalog. O chamador (Orders) fornece o preço-base obtido do Catalog.

## Estrutura

```text
src/
  FiapCloudGames.Promotions.Api/             composition root e pipeline
  FiapCloudGames.Promotions.Application/     casos de uso e portas técnicas
  FiapCloudGames.Promotions.Domain/          agregados e regras do domínio
  FiapCloudGames.Promotions.Infrastructure/  EF Core, repositories e autenticação
  FiapCloudGames.Promotions.Presentation/    Controllers, Requests, Responses e mappings HTTP
  FiapCloudGames.Promotions.Migrations/      migrations e atualizador do banco
tests/
  FiapCloudGames.Promotions.ArchitectureTests/
  FiapCloudGames.Promotions.Database.IntegrationTests/
  FiapCloudGames.Promotions.IntegrationTests/
  FiapCloudGames.Promotions.UnitTests/
```

## Pré-requisitos

- .NET SDK 10;
- PostgreSQL 17 (ou usar Docker Compose);
- Docker e Docker Compose (opcional, para execução via contêiner).

### Building Blocks

O pacote `FiapCloudGames.BuildingBlocks` versão `0.2.2` está disponível no
GitHub Packages como pacote privado. Este repositório inclui uma cópia local
em `packages/` como fallback, configurada no `NuGet.Config`. O restore funciona
sem autenticação no GitHub Packages enquanto o fallback estiver presente.

Para restaurar diretamente do GitHub Packages (sem fallback), defina:

```powershell
$env:GITHUB_PACKAGES_USERNAME = "MarinsPedro"
$env:GITHUB_PACKAGES_TOKEN = "SEU_PAT_CLASSIC"
```

O token precisa de `read:packages`. Remova-o da sessão quando não for mais
necessário:

```powershell
Remove-Item Env:\GITHUB_PACKAGES_TOKEN
```

## Execução local com Docker Compose (recomendado)

A forma mais simples de rodar o serviço completo:

```powershell
docker compose up --build
```

Isso sobe automaticamente:

| Serviço | Container | Porta |
|---|---|---|
| PostgreSQL 17 | `fcg-promotions-db` | `5434` |
| Migrations | `fcg-promotions-migrations` | — (executa e encerra) |
| API | `fcg-promotions-api` | `5084` |

A senha padrão do banco é `DevPass123!`. Para alterar, defina a variável
`POSTGRES_PASSWORD` antes de subir:

```powershell
$env:POSTGRES_PASSWORD = "SuaSenhaAqui"
docker compose up --build
```

Após subir, o Swagger fica disponível em `http://localhost:5084/swagger`.

Para derrubar tudo e limpar os volumes:

```powershell
docker compose down -v
```

## Execução local sem Docker

1. Inicie um PostgreSQL e crie o banco:

```sql
CREATE USER fcg_promotions_app WITH PASSWORD 'SuaSenha';
CREATE DATABASE fcg_promotions OWNER fcg_promotions_app;
```

2. Configure a connection string:

```powershell
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fcg_promotions;Username=fcg_promotions_app;Password=SuaSenha"
```

3. Configure a autenticação JWT (apontando para o Identity Service):

```powershell
$env:Authentication__Authority = "http://localhost:5081"
$env:Authentication__Audience = "promotions-api"
```

4. Restaure, aplique as migrations e execute:

```powershell
dotnet tool restore
dotnet restore FiapCloudGames.Promotions.slnx
dotnet run --project src/FiapCloudGames.Promotions.Migrations
dotnet run --project src/FiapCloudGames.Promotions.Api
```

Em `Development`, o Swagger fica disponível em `http://localhost:5083/swagger`.

## Configuração

| Variável | Descrição | Exemplo |
|---|---|---|
| `ConnectionStrings__Database` | Connection string do PostgreSQL | `Host=localhost;Port=5432;Database=fcg_promotions;Username=fcg_promotions_app;Password=xxx` |
| `Authentication__Authority` | URL do Identity Service (emissor dos tokens) | `http://localhost:5081` |
| `Authentication__Audience` | Audience esperada nos tokens JWT | `promotions-api` |
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicação | `Development` |

## Criando migrations

```powershell
dotnet ef migrations add NomeDaMigration `
  --project src/FiapCloudGames.Promotions.Migrations `
  --startup-project src/FiapCloudGames.Promotions.Migrations `
  --output-dir Migrations
```

O processo da API nunca executa migrations automaticamente. Em contêineres, use
a imagem com target `migrations` antes de iniciar a API.

## Endpoints principais

| Método | Rota | Autenticação | Uso |
|---|---|---|---|
| `POST` | `/internal/v1/campaigns` | service + `promotions.campaigns.write` | cria campanha |
| `GET` | `/internal/v1/campaigns/{campaignId}` | service + `promotions.campaigns.read` | consulta campanha |
| `DELETE` | `/internal/v1/campaigns/{campaignId}` | service + `promotions.campaigns.write` | desativa campanha |
| `POST` | `/internal/v1/promotions/calculate` | service + `promotions.calculate` | calcula desconto |
| `GET` | `/health/live` | anônimo | liveness |
| `GET` | `/health/ready` | anônimo | readiness |

Todos os endpoints de negócio são autenticados por padrão. Somente health checks
aceitam chamadas anônimas.

## Contratos dos endpoints de campanha

### Criar campanha — `POST /internal/v1/campaigns`

**Request:**

```json
{
  "name": "Promoção de Inverno",
  "discountType": 1,
  "discountValue": 15.00,
  "startDateUtc": "2026-10-01T00:00:00Z",
  "endDateUtc": "2026-10-31T23:59:59Z",
  "gameId": "80fdc742-e7c1-4564-a269-8d52784bec44"
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `name` | string | Nome da campanha (3 a 200 caracteres) |
| `discountType` | int | `1` = Fixed, `2` = Percentage |
| `discountValue` | decimal | Valor do desconto. Fixo: máximo `9999999999.99`. Percentual: máximo `100` |
| `startDateUtc` | datetime UTC | Início da vigência |
| `endDateUtc` | datetime UTC | Fim da vigência (deve ser posterior ao início) |
| `gameId` | UUID? | Jogo específico elegível. `null` = todos os jogos |

**Response (201 Created):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Promoção de Inverno",
  "discountType": "Fixed",
  "discountValue": 15.00,
  "startDateUtc": "2026-10-01T00:00:00+00:00",
  "endDateUtc": "2026-10-31T23:59:59+00:00",
  "gameId": "80fdc742-e7c1-4564-a269-8d52784bec44",
  "isActive": true,
  "createdAtUtc": "2026-10-14T01:30:00+00:00"
}
```

### Consultar campanha — `GET /internal/v1/campaigns/{campaignId}`

**Response (200 OK):** mesmo formato do `201 Created` acima.

Retorna `404` se a campanha não existir.

### Desativar campanha — `DELETE /internal/v1/campaigns/{campaignId}`

**Response:** `204 No Content`.

Retorna `404` se a campanha não existir.

## Contrato de cálculo de desconto

O endpoint `POST /internal/v1/promotions/calculate` (`PostCalcularDesconto`) é o
contrato principal consumido pelo Orders Service durante o fluxo de compra.

**Request:**

```json
{
  "gameId": "80fdc742-e7c1-4564-a269-8d52784bec44",
  "basePrice": 99.90,
  "promotionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `gameId` | UUID | Identificador do jogo (obtido do Catalog) |
| `basePrice` | decimal | Preço-base do jogo (obtido do Catalog). Máximo `9999999999.99`, arredondado para 2 casas |
| `promotionId` | UUID | Identificador da promoção (campanha) a ser aplicada |

**Response (200 OK):**

```json
{
  "finalPrice": 84.90
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `finalPrice` | decimal | Preço final com desconto aplicado (mínimo `0`) |

**Respostas de erro:**

| Status | Cenário |
|---|---|
| `400` | `basePrice` negativo ou acima do teto |
| `404` | `promotionId` não encontrada |
| `422` | Promoção inativa, fora do período de vigência, ou não elegível para o jogo |

## Qualidade

```powershell
dotnet build FiapCloudGames.Promotions.slnx -c Release
dotnet test FiapCloudGames.Promotions.slnx -c Release --no-build
dotnet format FiapCloudGames.Promotions.slnx --verify-no-changes --no-restore
```

## Segurança operacional

- armazene segredos e connection strings em variáveis de ambiente ou secret manager, nunca em Git;
- não registre tokens JWT, segredos ou connection strings em logs;
- revogue imediatamente qualquer token pessoal do GitHub que tenha sido exposto;
- substitua as credenciais locais conhecidas fora de `Development`.
