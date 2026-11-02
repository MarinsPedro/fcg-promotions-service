using FiapCloudGames.BuildingBlocks.Hosting;
using FiapCloudGames.BuildingBlocks.Web;
using FiapCloudGames.Promotions.Api.Configuration;
using FiapCloudGames.Promotions.Infrastructure;
using FiapCloudGames.Promotions.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.AddFiapServiceDefaults();
builder.Services.AddFiapApiDefaults();
builder.Services.AddPromotionsInfrastructure(builder.Configuration);
builder.Services.AddPromotionsPresentation();
builder.Services.AddPromotionsOpenApi();

var app = builder.Build();

app.UseFiapApiDefaults();
app.UsePromotionsOpenApi();
app.UseAuthentication();
app.UseAuthorization();
app.MapFiapHealthEndpoints();
app.MapControllers();

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
