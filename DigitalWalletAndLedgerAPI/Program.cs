using DigitalWalletAndLedgerAPI.Extensions;
using DigitalWalletAndLedgerAPI.Helpers;
using DigitalWalletAndLedgerAPI.Installers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) =>
{
    config.MinimumLevel.Information()
          .WriteTo.Console()
          .WriteTo.File("Logs/wallet-api-log-.txt", rollingInterval: RollingInterval.Day);
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthAndSecurity(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapDevelopmentEndpoints();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.InitializeDatabase();
app.MapWalletEndpoints();

app.Run();