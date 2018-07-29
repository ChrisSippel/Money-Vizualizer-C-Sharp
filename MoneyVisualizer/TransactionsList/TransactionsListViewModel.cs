using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using MoneyVisualizer.Helpers;
using MoneyVisualizer.Helpers.EventHandlers;

namespace MoneyVisualizer.TransactionsList
{
    public sealed class TransactionsListViewModel : NotifyPropertyChanged
    {
        private const string NoFilter = "Nothing";
        private readonly IReadOnlyList<ITransaction> _transactions;
        private IReadOnlyList<TransactionView> _transactionViews;
        private string _filter;

        public TransactionsListViewModel(ObservableCollection<ITransaction> transactions)
        {
            _transactions = transactions;
            _filter = NoFilter;

            foreach (var transaction in transactions)
            {
                transaction.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(OnPropertyChanged).Handler;
            }

            transactions.CollectionChanged += TransactionsOnCollectionChanged;

            UpdateTransactionsList();

            var filters = new List<string> { NoFilter };
            filters.AddRange(CategoryTypes.List);

            Filters = filters;
        }

        public IEnumerable<string> Filters { get; }

        public string CurrentFilter
        {
            get
            {
                return _filter;
            }

            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    OnPropertyChanged();
                    UpdateTransactionsList();
                }
            }
        }

        private void UpdateTransactionsList()
        {
            var transactionViews = new List<TransactionView>();
            for (int i = 0; i < _transactions.Count; i++)
            {
                var transaction = (Transaction)_transactions[i];
                if (CurrentFilter != NoFilter &&
                    transaction.Category != CurrentFilter)
                {
                    continue;
                }

                var transactionViewModel = new TransactionViewModel(transaction);
                var transactionView = new TransactionView
                {
                    DataContext = transactionViewModel,
                };

                transactionViews.Add(transactionView);
            }

            Transactions = transactionViews;
        }

        public IReadOnlyList<TransactionView> Transactions
        {
            get
            {
                return  _transactionViews;
            }

            set
            {
                if (_transactionViews != null &&
                    _transactionViews.Equals(value))
                {
                    return;
                }

                _transactionViews = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UpdateTransactionsList();
        }

        private void TransactionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateTransactionsList();

            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(ITransaction newItem in notifyCollectionChangedEventArgs.NewItems)
                {
                    newItem.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(OnPropertyChanged).Handler;
                }
            }
        }
    }
}
