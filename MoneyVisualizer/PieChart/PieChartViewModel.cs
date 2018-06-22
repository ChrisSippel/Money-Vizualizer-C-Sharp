using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using MoneyVisualizer.Helpers.EventHandlers;

namespace MoneyVisualizer.PieChart
{
    public sealed class PieChartViewModel
    {
        private IReadOnlyList<ITransaction> _transactions;
        private Func<ChartPoint, string> _pointLabel;

        public PieChartViewModel(IReadOnlyList<ITransaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                transaction.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(OnPropertyChanged).Handler;
            }

            _pointLabel = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})";
            PointLabel = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})";

            _transactions = transactions;
            Series = new SeriesCollection();

            LoadTransactionsIntoPieChart();
        }

        public Func<ChartPoint, string> PointLabel { get; set; }

        public SeriesCollection Series { get; set; }

        private void LoadTransactionsIntoPieChart()
        {
            Series.Clear();
            var transactionsByCategory = _transactions.GroupBy(x => x.Category);
            foreach (var transactionCategory in transactionsByCategory)
            {
                Series.Add(new PieSeries
                {
                    Title = transactionCategory.Key,
                    Values = new ChartValues<decimal> { transactionCategory.Where(x => x.Value < 0).Sum(x => x.Value) * -1 },
                    LabelPoint = _pointLabel,
                    DataLabels = true
                });
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            LoadTransactionsIntoPieChart();
        }
    }
}
