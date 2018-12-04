using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MoneyVisualizer.TransactionsList
{
    public interface ITransactionViewModel : INotifyPropertyChanged
    {
        IEnumerable<string> Categories { get; }

        string Category { get; set; }

        DateTime DateTime { get; }

        string Description { get; set; }

        decimal Value { get; }

        string Vendor { get; set; }
    }
}