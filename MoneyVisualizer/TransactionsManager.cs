using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoneyVisualizer.ExtensionMethods;
using System.Collections.ObjectModel;

namespace MoneyVisualizer
{
    public sealed class TransactionsManager : ITransactionsManager
    {
        private readonly ITransactionFactory _transactionsFactory;

        public TransactionsManager(ITransactionFactory transactionsFactory)
        {
            _transactionsFactory = transactionsFactory;
        }

        public IEnumerable<ITransaction> CreateTransactionsFromFile(string filePath, SupportedTransactionTypes transactionsType)
        {
            var stringTransactions = LoadTransactionsFromFile(filePath);

            ICollection<ITransaction> transactions = new List<ITransaction>();

            stringTransactions.ForEach(x =>
            {
                var transaction = _transactionsFactory.CreateTransaction(x, transactionsType);
                if (transaction != NoneTransaction.Instance)
                {
                    transactions.Add(transaction);
                }
            });

            return transactions;
        }

        /// <inheritdoc />
        public void ReplaceWithTransactionsFromFile(
            Func<ITransaction, bool> toReplaceFunc,
            string filePath,
            ObservableCollection<ITransaction> transactions,
            SupportedTransactionTypes transactionsType)
        {
            var transactionsToReplace = new List<ITransaction>(transactions.Where(x => toReplaceFunc(x)));
            if (!transactionsToReplace.Any(x => toReplaceFunc(x)) ||
                string.IsNullOrWhiteSpace(filePath) ||
                !File.Exists(filePath))
            {
                // There are no transactions to replace, or the given file path is empty or doesn't exist.
                // Return the original collection.
                return;
            }

            foreach (var transactionToReplace in transactionsToReplace)
            {
                transactions.Remove(transactionToReplace);
            }

            var transactionsByDate = transactions.GroupBy(x => x.DateTime);
            var replacementTransactions = CreateTransactionsFromFile(filePath, transactionsType);

            // Find the Date/Time grouping the replacement transactions belong to, and add them to that grouping.
            foreach (var replacementTransaction in replacementTransactions)
            {
                IGrouping<DateTime, ITransaction> transactionsForDate = null;
                if (transactionsByDate.Any(x => x.Key.Equals(replacementTransaction.DateTime)))
                {
                    transactionsForDate = transactionsByDate.Last(x => x.Key.Equals(replacementTransaction.DateTime));
                }
                else
                {
                    var dateTimeToCheckFor = replacementTransaction.DateTime.AddDays(-1);
                    while (transactionsForDate == null)
                    {
                        if (transactionsByDate.Any(x => x.Key.Equals(dateTimeToCheckFor)))
                        {
                            transactionsForDate = transactionsByDate.Last(x => x.Key.Equals(dateTimeToCheckFor));
                            break;
                        }

                        dateTimeToCheckFor = dateTimeToCheckFor.AddDays(-1);
                    }
                }

                var lastTransactionForDate = transactionsForDate.Last();


                // use '+' because all of the values will be negative
                var newAccountBalance = lastTransactionForDate.AccountBalance + replacementTransaction.Value;

                // Make a new Transaction because Transactions are immutable
                var newTransaction = new Transaction(
                    replacementTransaction.DateTime,
                    replacementTransaction.Vendor,
                    replacementTransaction.Value,
                    newAccountBalance,
                    replacementTransaction.Description,
                    replacementTransaction.Category);

                int indexOfLastTransaction = transactions.IndexOf(lastTransactionForDate);
                if (indexOfLastTransaction == transactions.Count)
                {
                    transactions.Add(newTransaction);
                }
                else
                {
                    transactions.Insert(indexOfLastTransaction + 1, newTransaction);
                }
            }
        }

        /// <summary>
        /// Loads the string transactions from the given file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>A colection of string transactions from the given file.</returns>
        private IEnumerable<string> LoadTransactionsFromFile(string filePath)
        {
            var transactions = new List<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        transactions.Add(line);
                    }
                }
            }

            return transactions;
        }
    }
}
