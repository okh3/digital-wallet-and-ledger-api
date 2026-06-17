using DigitalWalletAndLedgerAPI.Models;
using DigitalWalletAndLedgerAPI.Services.Data;
using MediatR;
using System;

namespace DigitalWalletAndLedgerAPI.Services.Transactions;

public record GetWalletBalanceQuery(Guid WalletId) : IRequest<decimal>;

public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, decimal>
{
    private readonly IWalletRepository _repository;

    public GetWalletBalanceQueryHandler(IWalletRepository repository)
    {
        _repository = repository;
    }

    public async Task<decimal> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
    {
        var wallet = await _repository.GetByIdAsync(request.WalletId);

        if (wallet == null) throw new WalletNotFoundException("Wallet not found.");

        return wallet.Balance;
    }
}