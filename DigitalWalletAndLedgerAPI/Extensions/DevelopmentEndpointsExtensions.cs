using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DigitalWalletAndLedgerAPI.Config;
using DigitalWalletAndLedgerAPI.Services.Data;
using Microsoft.IdentityModel.Tokens;

namespace DigitalWalletAndLedgerAPI.Extensions;

public static class DevelopmentEndpointsExtensions
{
    public static void MapDevelopmentEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/login", (IConfiguration config, ILogger<Program> logger) =>
        {
            logger.LogInformation("Development Endpoint Hit: Generating test JWT token.");

            var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            logger.LogDebug("Successfully generated test token for Issuer: {Issuer}", jwtSettings.Issuer);

            return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        })
        .WithTags("Test Utilities")
        .WithSummary("Development Only: Generate a JWT Token");


        app.MapPost("/api/wallet/test-seed", async (IWalletRepository repo, ILogger<Program> logger) =>
        {
            var randomUserId = Guid.NewGuid();
            
            logger.LogInformation("Development Endpoint Hit: Seeding test wallet for User ID: {UserId}", randomUserId);

            var wallet = new Models.Wallet(randomUserId);
            wallet.Deposit(1000m);

            await repo.AddWalletAsync(wallet);
            await repo.SaveChangesAsync();

            logger.LogInformation("Successfully created test Wallet ID: {WalletId} with starting balance $1000", wallet.Id);

            return Results.Ok(new { message = "Test Wallet Created", walletId = wallet.Id, startingBalance = wallet.Balance });
        })
        .WithTags("Test Utilities")
        .WithSummary("Development Only: Create a test wallet with $1000");
    }
}