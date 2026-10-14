# Estratégia de testes

Esta é a regra de testes do Promotions Service. Cada comportamento deve ser
validado na suíte mais interna e rápida capaz de comprová-lo.

## Matriz das suítes

| Projeto | O que comprova | Pode depender de |
|---|---|---|
| `Promotions.UnitTests` | Invariantes do Domain e validações de valor | Domain |
| `Promotions.IntegrationTests` | Host real, pipeline HTTP, contratos e segurança | API e camadas externas |
| `Promotions.Database.IntegrationTests` | Modelo relacional, metadata Npgsql e conventions | Infrastructure e Migrations |
| `Promotions.ArchitectureTests` | Direção das dependências e localização de responsabilidades | Assemblies inspecionados |

## UnitTests

```text
UnitTests/
  Domain/
    Campaigns/
      CampaignTests.cs
```

Regras obrigatórias:

- Referenciar somente o projeto `Domain`.
- Não usar Infrastructure, Presentation, API, Migrations, EF Core ou
  `WebApplicationFactory`.
- Testar invariantes da entidade, value objects e transições de estado.
- Usar dados determinísticos.
- Verificar que falhas lançam as exceções corretas.

Os testes cobrem:

- Criação válida de campanha.
- Validações de nome, desconto, datas e percentual.
- Deactivate.
- Cálculo de desconto fixo e percentual.
- Limite máximo de valor e arredondamento.

Formato de nome: `Metodo_Cenario_ResultadoEsperado`.

## IntegrationTests

```text
IntegrationTests/
  Host/
    HealthEndpointTests.cs
  Contracts/
    CampaignEndpointTests.cs
    OpenApiContractTests.cs
  Support/
    PromotionsApiFactory.cs
```

- `Host`: health checks e comportamento global do processo.
- `Contracts`: status code, rotas, autorização e contrato OpenAPI.
- `Support`: `WebApplicationFactory` com `TestAuthHandler` e fixtures.

Testes por endpoint verificam comportamento HTTP e de segurança. Regras de
negócio não devem ser testadas exclusivamente através da API.

O banco em memória é usado pelo host HTTP para manter os testes rápidos.

## Database.IntegrationTests

Essa suíte inspeciona o modelo configurado com o provider Npgsql e o assembly
de migrations. Ela valida:

- Conventions do EF Core aplicadas corretamente.
- Metadata do modelo relacional.

Esses testes não abrem conexão com um PostgreSQL real.

## ArchitectureTests

As regras automatizadas falham quando:

- Uma camada interna referencia uma camada externa.
- Controllers aparecem fora de Presentation.
- Migrations aparecem fora do projeto Migrations.
- UnitTests ganha referência de projeto além de Domain.

## Execução local

```powershell
dotnet build FiapCloudGames.Promotions.slnx -c Release
dotnet test FiapCloudGames.Promotions.slnx -c Release --no-build
dotnet format FiapCloudGames.Promotions.slnx --verify-no-changes --no-restore
```

Durante o desenvolvimento, execute primeiro a suíte diretamente afetada. Antes
de entregar, execute a solução completa.
