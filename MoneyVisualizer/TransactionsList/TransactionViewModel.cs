using System;
using System.Collections.Generic;
using System.Linq;
using MoneyVisualizer.Helpers;

namespace MoneyVisualizer.TransactionsList
{
    public sealed class TransactionViewModel : NotifyPropertyChanged, ITransactionViewModel
    {
        private readonly ITransaction _transaction;

        public TransactionViewModel(ITransaction transaction)
        {
            _transaction = transaction;
            Category = !CategoryTypes.List.Contains(transaction.Category)
                ? CategoryTypes.List.First()
                : transaction.Category;
        }

        public IEnumerable<string> Categories { get; } = CategoryTypes.List;

        public DateTime DateTime =>_transaction.DateTime;

        public decimal Value => _transaction.Value;

        public string Vendor
        {
            get
            {
                return _transaction.Vendor;
            }

            set
            {
                if (_transaction.Vendor == value)
                {
                    return;
                }

                _transaction.Vendor = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return _transaction.Description;
            }

            set
            {
                if (_transaction.Description == value)
                {
                    return;
                }

                _transaction.Description = value;
                OnPropertyChanged();
            }
        }

        public string Category
        {
            get
            {
                return _transaction.Category;
            }

            set
            {
                if (_transaction.Category == value)
                {
                    return;
                }

                _transaction.Category = value;
                OnPropertyChanged();
            }
        }
    }
}
