using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyVisualizer.TransactionsList
{
    public sealed class TransactionsListViewModel
    {
        public TransactionsListViewModel(IReadOnlyList<ITransaction> transactions)
        {
            var transactionViews = new TransactionView[transactions.Count];
            for (int i = 0; i < transactions.Count; i++)
            {
                var transaction = (DebitTransaction)transactions[i];
                var transactionViewModel = new TransactionViewModel(transaction);
                var transactionView = new TransactionView
                {
                    DataContext = transactionViewModel,
                };

                transactionViews[i] = transactionView;
            }

            Transactions = transactionViews;
        }

        public IReadOnlyList<TransactionView> Transactions { get; }
    }
}
