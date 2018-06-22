using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyVisualizer.QuickInfoPage
{
    public sealed class QuickInfoPageViewModel
    {
        public QuickInfoPageViewModel(IReadOnlyList<ITransaction> transactions)
        {
            var debitTransactions = GetDebitTransactionsFromRawTransactions(transactions);

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

        private List<DebitTransaction> GetDebitTransactionsFromRawTransactions(IReadOnlyList<ITransaction> transactions)
        {
            List<DebitTransaction> transactionsList = transactions.Cast<DebitTransaction>().ToList();
            return transactionsList;
        }
    }
}
