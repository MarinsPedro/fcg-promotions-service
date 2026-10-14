# Endpoints e contratos do Promotions

Este documento detalha cada endpoint do Promotions Service, quem deve chamá-lo
e os contratos de request/response.

## Visão rápida

| Endpoint | Quem chama | Credencial exigida | Finalidade |
|---|---|---|---|
| `POST /internal/v1/campaigns` | administração interna | service + `promotions.campaigns.write` | criar campanha |
| `GET /internal/v1/campaigns/{campaignId}` | administração interna | service + `promotions.campaigns.read` | consultar campanha |
| `DELETE /internal/v1/campaigns/{campaignId}` | administração interna | service + `promotions.campaigns.write` | desativar campanha |
| `POST /internal/v1/promotions/calculate` | Orders Service | service + `promotions.calculate` | calcular desconto |
| `GET /health/live` | infraestrutura | nenhuma | liveness |
| `GET /health/ready` | infraestrutura | nenhuma | readiness |

Todos os endpoints de negócio são autenticados por padrão. Somente health checks
aceitam chamadas anônimas.

## Convenção das URLs

- `/internal/v1` identifica contratos entre aplicações da plataforma.
- `/health` são contratos de infraestrutura e não recebem versão.

O segmento `internal` comunica a intenção da rota, mas não é um mecanismo de
segurança. Essas rotas continuam protegidas por JWT de serviço, audience,
scopes e controles de rede.

## Pré-requisitos

Antes de testar os fluxos:

1. Inicie o Identity Service.
2. Obtenha um token de serviço com as audiences e scopes necessários.
3. Inicie o Promotions Service.

Para obter um token de serviço do Identity:

```http
POST /oauth2/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials&client_id=web-bff&client_secret=SEU_SEGREDO&audience=promotions-api&scope=promotions.campaigns.write promotions.campaigns.read promotions.calculate
```

## Detalhamento dos endpoints

### `POST /internal/v1/campaigns`

Cria uma campanha promocional. Exige token `service` com scope
`promotions.campaigns.write`.

```http
POST /internal/v1/campaigns
Authorization: Bearer <token-service>
Content-Type: application/json

{
  "name": "Promoção de Inverno",
  "discountType": 1,
  "discountValue": 15.00,
  "startDateUtc": "2026-10-01T00:00:00Z",
  "endDateUtc": "2026-10-31T23:59:59Z",
  "gameId": "80fdc742-e7c1-4564-a269-8d52784bec44"
}
```

| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `name` | string | sim | Nome da campanha (3 a 200 caracteres) |
| `discountType` | int | sim | `1` = Fixed (valor absoluto), `2` = Percentage |
| `discountValue` | decimal | sim | Fixo: máximo `9999999999.99`. Percentual: máximo `100` |
| `startDateUtc` | datetime | sim | Início da vigência (UTC) |
| `endDateUtc` | datetime | sim | Fim da vigência (deve ser posterior ao início) |
| `gameId` | UUID? | não | Jogo específico elegível. `null` = todos os jogos |

Resposta `201 Created`:

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

O header `Location` aponta para o endpoint de consulta.

### `GET /internal/v1/campaigns/{campaignId}`

Consulta uma campanha pelo ID. Exige token `service` com scope
`promotions.campaigns.read`.

```http
GET /internal/v1/campaigns/3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer <token-service>
```

Retorna `200 OK` com o mesmo formato da criação ou `404 Not Found` quando o
ID não existe.

### `DELETE /internal/v1/campaigns/{campaignId}`

Desativa logicamente a campanha; nenhum registro é removido do banco. Exige
token `service` com scope `promotions.campaigns.write`.

```http
DELETE /internal/v1/campaigns/3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer <token-service>
```

Retorna `204 No Content` ou `404 Not Found`.

Uma campanha desativada não é mais elegível para cálculo de desconto.

### `POST /internal/v1/promotions/calculate` (PostCalcularDesconto)

Calcula o desconto de uma promoção específica para um jogo. Contrato principal
consumido pelo Orders Service durante o fluxo de compra. Exige token `service`
com scope `promotions.calculate`.

```http
POST /internal/v1/promotions/calculate
Authorization: Bearer <token-service>
Content-Type: application/json

{
  "gameId": "80fdc742-e7c1-4564-a269-8d52784bec44",
  "basePrice": 99.90,
  "promotionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `gameId` | UUID | Identificador do jogo (obtido do Catalog) |
| `basePrice` | decimal | Preço-base do jogo. Máximo `9999999999.99`, arredondado para 2 casas |
| `promotionId` | UUID | Identificador da promoção (campanha) a ser aplicada |

Resposta `200 OK`:

```json
{
  "finalPrice": 84.90
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `finalPrice` | decimal | Preço final com desconto aplicado (mínimo `0`) |

### Respostas de erro do calculate

| Status | Cenário |
|---|---|
| `400 Bad Request` | `basePrice` negativo ou acima de `9999999999.99` |
| `404 Not Found` | `promotionId` não encontrada |
| `422 Unprocessable Entity` | Promoção inativa |
| `422 Unprocessable Entity` | Promoção fora do período de vigência |
| `422 Unprocessable Entity` | Promoção não elegível para o jogo informado |

Os erros são retornados como `application/problem+json` pelo middleware dos
Building Blocks.

## Respostas de erro comuns

| Status | Significado típico |
|---|---|
| `400 Bad Request` | Contrato inválido ou valor fora do range |
| `401 Unauthorized` | Token ausente, expirado ou inválido |
| `403 Forbidden` | Token válido mas sem audience ou scope necessário |
| `404 Not Found` | Campanha ou promoção não encontrada |
| `422 Unprocessable Entity` | Regra de negócio violada |
