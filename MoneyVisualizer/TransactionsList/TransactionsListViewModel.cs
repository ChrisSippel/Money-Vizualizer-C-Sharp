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

        public IReadOnlyList<ITransactionViewModel> Transactions { get; private set; }

        private void UpdateTransactionsList()
        {
            var transactionViewModels = new List<TransactionViewModel>();
            for (int i = 0; i < _transactions.Count; i++)
            {
                var transaction = (Transaction)_transactions[i];
                if (CurrentFilter != NoFilter &&
                    transaction.Category != CurrentFilter)
                {
                    continue;
                }

                var transactionViewModel = new TransactionViewModel(transaction);
                transactionViewModels.Add(transactionViewModel);
            }

            Transactions = new ReadOnlyCollection<TransactionViewModel>(transactionViewModels);

            OnPropertyChanged(nameof(Transactions));
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
