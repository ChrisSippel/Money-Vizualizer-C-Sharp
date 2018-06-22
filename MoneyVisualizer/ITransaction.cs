using System;
using System.ComponentModel;
using MoneyVisualizer.Helpers;

namespace MoneyVisualizer
{
    /// <summary>
    /// The interface that represents a bank transaction
    /// </summary>
    public interface ITransaction : INotifyPropertyChanged
    {
        /// <summary>
        /// The date and time the transaction occurred.
        /// </summary>
        DateTime DateTime { get; }

        /// <summary>
        /// The description of the transaction.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The monetary value of the transaction.
        /// </summary>
        decimal Value { get; }

        /// <summary>
        /// The balance of the bank account minus the <see cref="Value"/> of the transaction.
        /// </summary>
        decimal AccountBalance { get; }

        /// <summary>
        /// The vendor the transaction happened with.
        /// </summary>
        string Vendor { get; set; }

        /// <summary>
        /// The category assigned to the transaction.
        /// </summary>
        string Category { get; set; }
    }

    /// <summary>
    /// An attempt to make instances of <see cref="ITransaction"/> not use null.
    /// </summary>
    public sealed class NoneTransaction : NotifyPropertyChanged, ITransaction
    {
        private NoneTransaction()
        {
        }

        public DateTime DateTime { get; } = DateTime.MinValue;

        public string Description
        {
            get { return string.Empty; }
            set { }
        }

        public decimal Value { get; } = decimal.MinValue;

        public decimal AccountBalance { get; } = decimal.MinValue;

        public string Vendor
        {
            get  { return string.Empty; }
            set { }
        }

        public string Category
        {
            get { return string.Empty; }
            set { }
        }

        public static NoneTransaction Instance { get; } = new NoneTransaction();
    }
}
