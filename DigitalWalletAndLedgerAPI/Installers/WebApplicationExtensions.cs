using DigitalWalletAndLedgerAPI.Services.Data;
using DigitalWalletAndLedgerAPI.Services.Transactions;
using DigitalWalletAndLedgerAPI.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DigitalWalletAndLedgerAPI.Installers
{
    public static class WebApplicationExtensions
    {
        public static void InitializeDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // In production the database migration would be moved to a standalone step in our CI/CD pipeline (like GitHub Actions or AWS CodePipeline)
                //so it runs exactly once before the new application instances start
                dbContext.Database.Migrate();

                logger.LogInformation("WebApplicationExtensions.InitializeDatabase: DATABASE CONNECTED & MIGRATED SUCCESSFULLY");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "WebApplicationExtensions.InitializeDatabase: DATABASE CONNECTION/MIGRATION FAILED!");
            }
        }

        public static void MapWalletEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/wallet")
                           .RequireAuthorization();

            app.MapPost("/api/wallet/transfer", async (TransferFundsCommand command, IMediator mediator, ILogger<Program> logger) =>
            {
                logger.LogInformation("Attempting transfer of ${Amount} from Wallet {FromWalletId} to Wallet {ToWalletId}",
                    command.Amount, command.FromWalletId, command.ToWalletId);

                var result = await mediator.Send(command);

                logger.LogInformation("Transfer successful!");
                return Results.Ok(new { message = "Transfer Successful", status = result });
            })
            .RequireAuthorization()
            .RequireRateLimiting("StrictPolicy")
            .AddEndpointFilter<IdempotencyFilter>();

            group.MapGet("/{id:guid}/balance", async (Guid id, IMediator mediator, ILogger<Program> logger) =>
            {
                logger.LogInformation("Fetching balance for Wallet ID: {WalletId}", id);
                var balance = await mediator.Send(new GetWalletBalanceQuery(id));
                logger.LogInformation("Successfully retrieved balance for Wallet ID: {WalletId}", id);

                return Results.Ok(new { walletId = id, balance = balance });
            });
        }
    }
}