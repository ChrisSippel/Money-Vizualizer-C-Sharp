using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoneyVisualizer.LineGraph
{
    public sealed class LineGraphViewModel
    {
        private readonly DateTimeAxis _dateTimeAxis;
        private readonly IntegerAxis _accountBalanceAxis;

        public LineGraphViewModel(IReadOnlyList<string> transactions,
            ITransactionFactory transactionFactory,
            DateTimeAxis dateTimeAxis,
            IntegerAxis accountBalanceAxis)
        {
            _dateTimeAxis = dateTimeAxis;
            _accountBalanceAxis = accountBalanceAxis;

            List<DebitTransaction> transactionsList = GetDebitTransactionsFromTransactions(transactions, transactionFactory);

            // Have to use doubles here instead of decimals because the charting controls need doubles.
            SortedDictionary<DateTime, double> accountBalanaceByDate = GetSortedTransactions(transactionsList);

            HandlePossibleFirstDayMissingTransaction(accountBalanaceByDate);

            PopulateMissingDates(accountBalanaceByDate, transactionsList);

            Data = GetChartData(accountBalanaceByDate);
        }

        public CompositeDataSource Data { get; }

        private List<DebitTransaction> GetDebitTransactionsFromTransactions(
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
