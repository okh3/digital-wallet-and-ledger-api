namespace DigitalWalletAndLedgerAPI.Models
{
    public class WalletNotFoundException : Exception
    {
        public WalletNotFoundException(string message) : base(message) { }
    }

    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(string message) : base(message) { }
    }
}
