using System;

namespace MoneyVisualizer
{
    /// <summary>
    /// An implementation of <see cref="ITransaction"/> that represents a debit transaction.
    /// </summary>
    public sealed class DebitTransaction : ITransaction
    {
        /// <summary>
        /// Constructs a new <see cref="DebitTransaction"/> object.
        /// </summary>
        /// <param name="dateTime">The date and time the transaction occurred.</param>
        /// <param name="description">The description of the transaction.</param>
        /// <param name="value">The value of the transaction.</param>
        /// <param name="accountBalanace">The balance of the account, after the transaction.</param>
        public DebitTransaction(DateTime dateTime,
            string description,
            decimal value,
            decimal accountBalanace)
        {
            DateTime = dateTime;
            Value = value;
            Description = description;
            AccountBalance = accountBalanace;
        }

        /// <inheritdoc />
        public DateTime DateTime { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public decimal Value { get; }

        /// <inheritdoc />
        public decimal AccountBalance { get; }
    }
}