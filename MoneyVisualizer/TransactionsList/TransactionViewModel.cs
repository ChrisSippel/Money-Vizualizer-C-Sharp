using System;
using MoneyVisualizer.Helpers.Ui;

namespace MoneyVisualizer.TransactionsList
{
    public sealed class TransactionViewModel : NotifyPropertyChanged
    {
        private string _vendor;
        private string _description;

        public TransactionViewModel(ITransaction transaction)
        {
            DateTime = transaction.DateTime;
            Value = transaction.Value;
            Vendor = transaction.Description;
            Description = string.Empty;
        }

        public DateTime DateTime { get; }

        public decimal Value { get; }

        public string Vendor
        {
            get
            {
                return _vendor;
            }

            set
            {
                if (_vendor == value)
                {
                    return;
                }

                _vendor = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                _description = value;
                OnPropertyChanged();
            }
        }
    }
}
