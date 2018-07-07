using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MoneyVisualizer.Helpers.EventHandlers;

namespace MoneyVisualizer.QuickInfoPage
{
    public sealed class QuickInfoPageViewModel
    {
        private readonly IReadOnlyList<ITransaction> _transactions;

        public QuickInfoPageViewModel(ObservableCollection<ITransaction> transactions)
        {
            _transactions = transactions;

            foreach (var transaction in transactions)
            {
                transaction.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(OnPropertyChanged).Handler;
            }

            transactions.CollectionChanged += TransactionsOnCollectionChanged;

            SetProperties();
        }

        public decimal StartingValue { get; private set; }

        public decimal EndingValue { get; private set; }

        public decimal EndingDifference { get; private set; }

        public double TotalSpent { get; private set; }

        public double TotalMade { get; private set; }

        private void SetProperties()
        {
            if (!_transactions.Any())
            {
                return;
            }

            StartingValue = _transactions.First().AccountBalance - _transactions.First().Value;
            EndingValue = _transactions.Last().AccountBalance;
            EndingDifference = EndingValue - StartingValue;
            TotalSpent = (double)_transactions.Where(x => x.Value < 0).Sum(x => x.Value) * -1;
            TotalMade = (double)_transactions.Where(x => x.Value > 0).Sum(x => x.Value);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            SetProperties();
        }

        private void TransactionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            SetProperties();
        }
    }
}
