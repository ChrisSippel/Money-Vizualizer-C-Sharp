using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyVisualizer.QuickInfoPage
{
    public sealed class QuickInfoPageViewModel
    {
        public QuickInfoPageViewModel(
            IReadOnlyCollection<string> transactions,
            ITransactionFactory transactionFactory)
        {
            var debitTransactions = GetDebitTransactionsFromRawTransactions((IReadOnlyList<string>)transactions, transactionFactory);

            StartingValue = debitTransactions.First().AccountBalance - debitTransactions.First().Value;
            EndingValue = debitTransactions.Last().AccountBalance;
            EndingDifference = EndingValue - StartingValue;
            TotalSpent = (double)debitTransactions.Where(x => x.Value < 0).Sum(x => x.Value) * -1;
            TotalMade = (double)debitTransactions.Where(x => x.Value > 0).Sum(x => x.Value);
        }

        public decimal StartingValue { get; }

        public decimal EndingValue { get; }

        public decimal EndingDifference { get; }

        public double TotalSpent { get; }

        public double TotalMade { get; }

        private List<DebitTransaction> GetDebitTransactionsFromRawTransactions(
            IReadOnlyList<string> transactions,
            ITransactionFactory transactionFactory)
        {
            List<DebitTransaction> transactionsList = new List<DebitTransaction>();
            foreach (string transactionLine in transactions)
            {
                DebitTransaction transaction = (DebitTransaction)transactionFactory.CreateDebitTransaction(transactionLine);
                transactionsList.Add(transaction);
            }

            return transactionsList;
        }
    }
}
