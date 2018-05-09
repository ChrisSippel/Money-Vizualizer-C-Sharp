using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace MoneyVisualizer.LineGraph
{
    /// <summary>
    /// The view model that represents the Line Graph view of the app.
    /// </summary>
    public sealed class LineGraphViewModel
    {
        private readonly DateTimeAxis _dateTimeAxis;
        private readonly IntegerAxis _accountBalanceAxis;

        /// <summary>
        /// Creates a new <see cref="LineGraphViewModel"/> object.
        /// </summary>
        /// <param name="transactions">The collections of transactions that have occurred, in string form.</param>
        /// <param name="transactionFactory">The <see cref="ITransactionFactory"/> that will convert the raw transactions into <see cref="ITransaction"/> objects.</param>
        /// <param name="dateTimeAxis">The <see cref="DateTimeAxis"/> to use for data conversion so DynamicDataDisplay (D3) can display the dates/times of the transactions.</param>
        /// <param name="accountBalanceAxis">The <see cref="IntegerAxis"/> to use for data conversion so DynamicDataDisplay (D3) can display the balances of the transactions.</param>
        public LineGraphViewModel(IReadOnlyList<string> transactions,
            ITransactionFactory transactionFactory,
            DateTimeAxis dateTimeAxis,
            IntegerAxis accountBalanceAxis)
        {
            _dateTimeAxis = dateTimeAxis;
            _accountBalanceAxis = accountBalanceAxis;

            List<DebitTransaction> transactionsList = GetDebitTransactionsFromRawTransactions(transactions, transactionFactory);

            // Have to use doubles here instead of decimals because the charting controls need doubles.
            SortedDictionary<DateTime, double> accountBalanaceByDate = GetSortedTransactions(transactionsList);

            HandlePossibleFirstDayMissingTransaction(accountBalanaceByDate);

            PopulateMissingDates(accountBalanaceByDate, transactionsList);

            Data = GetChartData(accountBalanaceByDate);
        }

        /// <summary>
        /// The <see cref="CompositeDataSource"/> that D3 needs to display the transactions on graphs.
        /// </summary>
        public CompositeDataSource Data { get; }

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

        private SortedDictionary<DateTime, double> GetSortedTransactions(IReadOnlyList<DebitTransaction> transactionsList)
        {
            // Have to use doubles here instead of decimals because the charting controls need doubles.
            SortedDictionary<DateTime, double> accountBalanaceByDate = new SortedDictionary<DateTime, double>();
            var orderedTransactions = transactionsList.GroupBy(x => x.DateTime);
            foreach (IGrouping<DateTime, DebitTransaction> orderedTransaction in orderedTransactions)
            {
                accountBalanaceByDate.Add(orderedTransaction.Key, Convert.ToDouble(orderedTransaction.Last().AccountBalance));
            }

            return accountBalanaceByDate;
        }

        private void HandlePossibleFirstDayMissingTransaction(SortedDictionary<DateTime, double> accountBalanaceByDate)
        {
            KeyValuePair<DateTime, double> firstDateWithAccountValue = accountBalanaceByDate.First();
            if (firstDateWithAccountValue.Key.Day == 1)
            {
                return;
            }

            DateTime firstDayOfMonth = new DateTime(
                firstDateWithAccountValue.Key.Year,
                firstDateWithAccountValue.Key.Month,
                1);

            accountBalanaceByDate.Add(firstDayOfMonth, firstDateWithAccountValue.Value);
        }

        private void PopulateMissingDates(
            SortedDictionary<DateTime, double> accountBalanaceByDate,
            List<DebitTransaction> transactionsList)
        {
            int year = transactionsList.First().DateTime.Year;
            int month = transactionsList.First().DateTime.Month;
            int maxDateValue = transactionsList.Max(x => x.DateTime).Day + 1;

            for (int i = 1; i < maxDateValue; i++)
            {
                DateTime date = new DateTime(year, month, i);
                if (accountBalanaceByDate.ContainsKey(date))
                {
                    continue;
                }
                else
                {
                    DateTime previousDate = new DateTime(year, month, i - 1);
                    accountBalanaceByDate.Add(date, accountBalanaceByDate[previousDate]);
                }
            }
        }

        private CompositeDataSource GetChartData(SortedDictionary<DateTime, double> accountBalanaceByDate)
        {
            var datesDataSource = new EnumerableDataSource<DateTime>(accountBalanaceByDate.Keys);
            datesDataSource.SetXMapping(x => _dateTimeAxis.ConvertToDouble(x));

            var accountBalanceDataSource = new EnumerableDataSource<double>(accountBalanaceByDate.Values);
            accountBalanceDataSource.SetYMapping(y => _accountBalanceAxis.ConvertFromDouble(y));

            return new CompositeDataSource(datesDataSource, accountBalanceDataSource);
        }
    }
}
