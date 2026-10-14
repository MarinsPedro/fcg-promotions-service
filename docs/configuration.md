# Configuração do Promotions

As configurações seguem a hierarquia padrão do ASP.NET Core. Em produção, use
variáveis de ambiente e um gerenciador de segredos.

## Banco de dados

```text
ConnectionStrings__Database=Host=localhost;Port=5432;Database=fcg_promotions;Username=fcg_promotions_app;Password=...
```

O Promotions é o único proprietário do banco `fcg_promotions`. Nenhum outro
serviço deve acessar diretamente suas tabelas.

O executável `FiapCloudGames.Promotions.Migrations` usa o Generic Host. Ele
carrega o `appsettings.json`, o arquivo específico do ambiente, variáveis de
ambiente e argumentos de linha de comando.

A API não executa migrations no startup. O migrador deve terminar com sucesso
antes do processo da API ou do rollout do deployment.

## Autenticação JWT

```text
Authentication__Authority=http://localhost:5081
Authentication__Audience=promotions-api
```

- `Authority` aponta para o Identity Service, que emite e assina os tokens.
- `Audience` identifica este serviço. Tokens sem essa audience são rejeitados.

A validação do JWT é local por assinatura, emissor, audiência e expiração. O
Promotions não chama o Identity a cada requisição — ele obtém a chave pública
via JWKS e a mantém em cache.

## Variáveis de ambiente

| Variável | Descrição | Exemplo |
|---|---|---|
| `ConnectionStrings__Database` | Connection string do PostgreSQL | `Host=localhost;Port=5432;Database=fcg_promotions;Username=fcg_promotions_app;Password=xxx` |
| `Authentication__Authority` | URL do Identity Service | `http://localhost:5081` |
| `Authentication__Audience` | Audience esperada nos tokens JWT | `promotions-api` |
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicação | `Development` |
| `GITHUB_PACKAGES_USERNAME` | Usuário para GitHub Packages (somente restore) | `MarinsPedro` |
| `GITHUB_PACKAGES_TOKEN` | PAT com `read:packages` (somente restore) | — |

## Health checks

- `/health/live` verifica somente se o processo HTTP está respondendo.
- `/health/ready` verifica se o PostgreSQL pode ser acessado.

## Produção

Antes de publicar:

- substitua todos os placeholders e senhas locais;
- injete connection strings e segredos por secret manager;
- habilite TLS;
- não registre tokens JWT ou connection strings em logs;
- conceda ao workflow acesso de leitura ao pacote privado Building Blocks.
