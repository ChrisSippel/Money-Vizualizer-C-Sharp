using System;
using MoneyVisualizer.Helpers;

namespace MoneyVisualizer
{
    /// <summary>
    /// An implementation of <see cref="ITransaction"/> that represents a debit transaction.
    /// </summary>
    public sealed class Transaction : NotifyPropertyChanged, ITransaction
    {
        private string _description;
        private string _vendor;
        private string _category;

        /// <summary>
        /// Constructs a new <see cref="Transaction"/> object.
        /// </summary>
        /// <param name="dateTime">The date and time the transaction occurred.</param>
        /// <param name="vendor">The vendor of the transaction.</param>
        /// <param name="value">The value of the transaction.</param>
        /// <param name="accountBalanace">The balance of the account, after the transaction.</param>
        /// <param name="description">The description of the transaction.</param>
        /// <param name="category">The category of the transaction.</param>
        public Transaction(DateTime dateTime,
            string vendor,
            decimal value,
            decimal accountBalanace,
            string description,
            string category)
        {
            DateTime = dateTime;
            Description = description;
            Value = value;
            AccountBalance = accountBalanace;
            Vendor = vendor;
            Category = category;
        }

        /// <inheritdoc />
        public DateTime DateTime { get; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public decimal Value { get; }

        /// <inheritdoc />
        public decimal AccountBalance { get; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public string Category
        {
            get
            {
                return _category;
            }

            set
            {
                if (_category == value)
                {
                    return;
                }

                _category = value;
                OnPropertyChanged();
            }
        }
    }
}