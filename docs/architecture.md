# Arquitetura do Promotions Service

Este documento descreve as regras de estrutura, camadas e convenções do
Promotions Service.

## Camadas

```text
Presentation  →  Application  →  Domain
     ↓                ↓
Infrastructure ───────┘
```

| Camada | Projeto | Responsabilidade |
|---|---|---|
| Api | `FiapCloudGames.Promotions.Api` | Composition root, pipeline HTTP e configuração |
| Application | `FiapCloudGames.Promotions.Application` | Casos de uso, portas técnicas e DTOs internos |
| Domain | `FiapCloudGames.Promotions.Domain` | Entidades, value objects e regras de negócio |
| Infrastructure | `FiapCloudGames.Promotions.Infrastructure` | EF Core, repositórios, health checks |
| Presentation | `FiapCloudGames.Promotions.Presentation` | Controllers, requests, responses e policies |
| Migrations | `FiapCloudGames.Promotions.Migrations` | Migrations EF Core e runner |

## Regras de dependência

- **Domain** não referencia nenhum outro projeto nem pacotes de infraestrutura.
- **Application** referencia apenas Domain.
- **Infrastructure** referencia Application e Domain.
- **Presentation** referencia Application e Domain (para enums e tipos compartilhados).
- **Api** referencia todas as camadas para composição.
- **Migrations** referencia Infrastructure para acessar o `DbContext`.

Essas regras são verificadas automaticamente pelo projeto `ArchitectureTests`.

## Convenções de código

### Entidades e agregados

- Construtores privados.
- Factory method estático `Create()` com validações.
- Propriedades somente-leitura com setters `private` ou `init`.
- Value objects como records quando aplicável.

### Services (Application)

- Uma classe por caso de uso, nomeada como verbo no infinitivo
  (`CalculatePromotionService`, `CreateCampaignService`).
- Recebem dependências por injeção no construtor (primary constructor).
- Retornam um `Result` record tipado.
- Lançam `AppException` do BuildingBlocks para erros de validação, not found
  e regra de negócio.

### Controllers (Presentation)

- Um controller por feature ou agregado.
- Rota base com versionamento: `internal/v1/<recurso>`.
- Services injetados via `[FromServices]`.
- `ProducesResponseType` declarado para cada status code possível.

### Repositories

- Interface no Domain (`ICampaignRepository`).
- Implementação na Infrastructure com EF Core.
- `FindByIdAsync` retorna `null` quando não encontra (nunca lança exceção).

## Configuração da solução

- `Directory.Build.props` centraliza `TargetFramework`, `Nullable`,
  `TreatWarningsAsErrors` e a flag `UseLocalBuildingBlocks`.
- `Directory.Packages.props` centraliza versões de pacotes (Central Package
  Management).
- `global.json` trava a versão do SDK.
