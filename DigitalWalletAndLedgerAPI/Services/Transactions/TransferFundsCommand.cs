using DigitalWalletAndLedgerAPI.Services.Data;
using DigitalWalletAndLedgerAPI.Models;
using MediatR;

namespace DigitalWalletAndLedgerAPI.Services.Transactions;

public record TransferFundsCommand(Guid FromWalletId, Guid ToWalletId, decimal Amount) : IRequest<bool>;

public class TransferFundsCommandHandler : IRequestHandler<TransferFundsCommand, bool>
{
    private readonly IWalletRepository _repository;
    private readonly ILogger<TransferFundsCommandHandler> _logger;

    public TransferFundsCommandHandler(IWalletRepository repository, ILogger<TransferFundsCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating transfer of ${Amount} from Wallet {FromWalletId} to Wallet {ToWalletId}",
            request.Amount, request.FromWalletId, request.ToWalletId);

        var fromWallet = await _repository.GetByIdAsync(request.FromWalletId);
        var toWallet = await _repository.GetByIdAsync(request.ToWalletId);

        if (fromWallet == null || toWallet == null)
        {
            _logger.LogWarning("Transfer failed. One or both wallets not found. FromWalletId: {FromWalletId}, ToWalletId: {ToWalletId}",
                request.FromWalletId, request.ToWalletId);

            throw new WalletNotFoundException("One or both wallets not found.");
        }

        fromWallet.Withdraw(request.Amount);
        toWallet.Deposit(request.Amount);

        var transaction = new Transaction(request.FromWalletId, request.ToWalletId, request.Amount);
        await _repository.AddTransactionAsync(transaction);

        await _repository.SaveChangesAsync();

        _logger.LogInformation("Successfully completed transfer of ${Amount}. Transaction recorded.", request.Amount);

        return true;
    }
}