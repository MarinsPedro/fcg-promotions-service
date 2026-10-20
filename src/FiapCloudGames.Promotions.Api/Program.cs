using FiapCloudGames.BuildingBlocks.Authentication;
using FiapCloudGames.BuildingBlocks.Hosting;
using FiapCloudGames.BuildingBlocks.Web;
using FiapCloudGames.Promotions.Api.Configuration;
using FiapCloudGames.Promotions.Infrastructure;
using FiapCloudGames.Promotions.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.AddFiapServiceDefaults();
builder.Services.AddFiapApiDefaults();
builder.Services.AddFiapServiceAuthentication(builder.Configuration);
builder.Services.AddPromotionsInfrastructure(builder.Configuration);
builder.Services.AddPromotionsPresentation();
builder.Services.AddPromotionsOpenApi();

var app = builder.Build();

app.UseFiapApiDefaults();
app.UsePromotionsOpenApi();
app.UseAuthentication();

#if DEBUG

// Verificação: middleware para logar se o usuário está autenticado e quais suas claims.
app.Use(async (context, next) =>
{
    var authenticated = context.User.Identity?.IsAuthenticated;
    var claims = context.User.Claims
        .Select(claim => new { claim.Type, claim.Value })
        .ToArray();

    await next();
});

#endif

app.UseAuthorization();
app.MapFiapHealthEndpoints();
app.MapControllers();

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
