# Fiap Cloud Games — Promotions Service

Serviço responsável por campanhas promocionais e cálculo de descontos da plataforma Fiap Cloud Games.

Este repositório é independente do monólito. Ele possui banco, migrations, pipeline e ciclo de entrega próprios.

## Responsabilidades

- cadastrar, consultar e desativar campanhas promocionais;
- descontos fixos ou percentuais;
- período de vigência e elegibilidade por jogo;
- cálculo do desconto sobre um preço-base recebido;
- retorno do preço final e da promoção aplicada.

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
- PostgreSQL;
- acesso ao pacote privado `FiapCloudGames.BuildingBlocks` versão `0.2.2` no GitHub Packages.

## Execução local

1. Copie `.env.example` para o mecanismo local de segredos de sua preferência.
2. Inicie um PostgreSQL e crie o banco `fcg_promotions` com usuário `fcg_promotions_app`.
3. Restaure, aplique as migrations e execute:

```powershell
$env:GITHUB_PACKAGES_USERNAME = "MarinsPedro"
$env:GITHUB_PACKAGES_TOKEN = "SEU_PAT_CLASSIC"

dotnet tool restore
dotnet restore FiapCloudGames.Promotions.slnx
dotnet run --project src/FiapCloudGames.Promotions.Migrations
dotnet run --project src/FiapCloudGames.Promotions.Api
```

Em `Development`, a interface Swagger fica disponível em `http://localhost:5083/swagger`.

## Endpoints principais

| Método | Rota | Autenticação | Uso |
|---|---|---|---|
| `POST` | `/internal/v1/campaigns` | service + `promotions.campaigns.write` | cria campanha |
| `GET` | `/internal/v1/campaigns/{id}` | service + `promotions.campaigns.read` | consulta campanha |
| `DELETE` | `/internal/v1/campaigns/{id}` | service + `promotions.campaigns.write` | desativa campanha |
| `POST` | `/internal/v1/promotions/calculate` | service + `promotions.calculate` | calcula desconto |
| `GET` | `/health/live` | anônimo | liveness |
| `GET` | `/health/ready` | anônimo | readiness |

## Qualidade

```powershell
dotnet build FiapCloudGames.Promotions.slnx -c Release
dotnet test FiapCloudGames.Promotions.slnx -c Release --no-build
dotnet format FiapCloudGames.Promotions.slnx --verify-no-changes --no-restore
```
