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
        private readonly IReadOnlyList<ITransaction> _transactions;
        private IReadOnlyList<TransactionView> _transactionViews;

        public TransactionsListViewModel(ObservableCollection<ITransaction> transactions)
        {
            _transactions = transactions;

            foreach (var transaction in transactions)
            {
                transaction.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(OnPropertyChanged).Handler;
            }

            transactions.CollectionChanged += TransactionsOnCollectionChanged;

            UpdateTransactionsList();
        }

        private void UpdateTransactionsList()
        {
            var transactionViews = new TransactionView[_transactions.Count];
            for (int i = 0; i < _transactions.Count; i++)
            {
                var transaction = (Transaction)_transactions[i];
                var transactionViewModel = new TransactionViewModel(transaction);
                var transactionView = new TransactionView
                {
                    DataContext = transactionViewModel,
                };

                transactionViews[i] = transactionView;
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
        }
    }
}
