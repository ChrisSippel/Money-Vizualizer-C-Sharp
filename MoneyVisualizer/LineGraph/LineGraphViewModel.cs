using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace MoneyVisualizer.LineGraph
{
    /// <summary>
    /// The view model that represents the Line Graph view of the app.
    /// </summary>
    public sealed class LineGraphViewModel
    {
        /// <summary>
        /// Creates a new <see cref="LineGraphViewModel"/> object.
        /// </summary>
        /// <param name="transactions">The collections of transactions that have occurred, in <see cref="ITransaction"/> form.</param>
        public LineGraphViewModel(IReadOnlyList<ITransaction> transactions)
        {
            LoadTransactionsIntoChart(transactions);
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private void LoadTransactionsIntoChart(IReadOnlyList<ITransaction> transactions)
        {
            var dayConfig = Mappers.Xy<DateModel>()
                .X(dateModel => dateModel.DateTime.ToFileTimeUtc())
                .Y(dateModel => dateModel.Value);

            var debitTransactions = GetDebitTransactionsFromRawTransactions(transactions);
            var sortedTransactions = GetSortedTransactions(debitTransactions);
            HandlePossibleFirstDayMissingTransaction(sortedTransactions);
            PopulateMissingDates(sortedTransactions, debitTransactions);

            var chartValues = new ChartValues<DateModel>();
            chartValues.Add(new DateModel
            {
                DateTime = sortedTransactions.First().Key.Subtract(TimeSpan.FromDays(1)),
                Value = (double)(debitTransactions.First().AccountBalance - debitTransactions.First().Value),
            });

            foreach (var transaction in sortedTransactions)
            {
                chartValues.Add(new DateModel
                {
                    DateTime = transaction.Key,
                    Value = transaction.Value,
                });
            }

            SeriesCollection = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Title = "Account Balance",
                    Values = chartValues
                },
            };

            YFormatter = value =>
            {
                var dateTime = DateTime.FromFileTimeUtc((long)value);
                return dateTime.ToShortDateString();
            };
        }

        private List<DebitTransaction> GetDebitTransactionsFromRawTransactions(IReadOnlyList<ITransaction> transactions)
        {
            List<DebitTransaction> transactionsList = transactions.Cast<DebitTransaction>().ToList();
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
    }
}
