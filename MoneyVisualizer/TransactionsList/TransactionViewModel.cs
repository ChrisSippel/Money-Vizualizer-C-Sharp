using System;
using System.Collections.Generic;
using System.Linq;
using MoneyVisualizer.Helpers;

namespace MoneyVisualizer.TransactionsList
{
    public sealed class TransactionViewModel : NotifyPropertyChanged
    {
        private readonly ITransaction _transaction;

        public TransactionViewModel(ITransaction transaction)
        {
            _transaction = transaction;
            Category = CategoryTypes.First();
        }

        public IEnumerable<string> CategoryTypes { get; } = new List<string>
        {
            "Unknown",
            "Savings",
            "Mortgage",
            "Bills",
            "Car payment",
            "Eating out",
            "Groceries",
            "Shopping",
            "Pets",
            "Alcohol"
        };

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
