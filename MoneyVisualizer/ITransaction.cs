using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyVisualizer
{
    /// <summary>
    /// The interface that represents a bank transaction
    /// </summary>
    public interface ITransaction
    {
        /// <summary>
        /// The date and time the transaction occurred.
        /// </summary>
        DateTime DateTime { get; }

        /// <summary>
        /// The description of the transaction.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The monetary value of the transaction.
        /// </summary>
        decimal Value { get; }
    }

    /// <summary>
    /// An attempt to make instances of <see cref="ITransaction"/> not use null.
    /// </summary>
    public sealed class NoneTransaction : ITransaction
    {
        private NoneTransaction()
        {
            DateTime = DateTime.MinValue;
            Description = string.Empty;
            Value = -1;
        }

        public DateTime DateTime { get; }

        public string Description { get; }

        public decimal Value { get; }

        public static NoneTransaction Instance { get; } = new NoneTransaction();
    }
}
