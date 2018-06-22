namespace MoneyVisualizer
{
    public interface ITransactionFactory
    {
        ITransaction CreateTransaction(string transaction);
    }
}
