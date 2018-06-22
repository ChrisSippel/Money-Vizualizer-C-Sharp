using System;

namespace MoneyVisualizer
{
    public enum SupportedTransactionTypes
    {
        TdBankAccount,
        TdCreditCard,
        CapitalOneCreditCard,
        MoneyVisualizer,
    }

    public sealed class TransactionsFactory : ITransactionFactory
    {
        public ITransaction CreateTransaction(string transaction, SupportedTransactionTypes transactionType)
        {
            if (string.IsNullOrWhiteSpace(transaction))
            {
                throw new ArgumentNullException(nameof(transaction), $"!string.IsNullOrWhiteSpace({nameof(transaction)})");
            }

            string[] sections = transaction.Split(',');
            switch (transactionType)
            {
                case SupportedTransactionTypes.TdBankAccount:
                    return CreateTdBankAccountTransaction(sections);

                case SupportedTransactionTypes.MoneyVisualizer:
                    return CreateMoneyVisualizerTransaction(sections);

                default:
                    return NoneTransaction.Instance;
            }
        }

        private static ITransaction CreateTdBankAccountTransaction(string[] sections)
        {
            const int dateTimeIndex = 0;
            const int descriptionIndex = 1;
            const int costValueIndex = 2;
            const int creditValueIndex = 3;
            const int accountBalanceIndex = 4;

            DateTime dateTime = DateTime.Parse(sections[dateTimeIndex]);
            string description = sections[descriptionIndex];

            decimal value;
            if (!string.IsNullOrWhiteSpace(sections[costValueIndex]))
            {
                decimal.TryParse(sections[costValueIndex], out value);
                value *= -1;
            }
            else
            {
                decimal.TryParse(sections[creditValueIndex], out value);
            }

            decimal accountBalance = decimal.Parse(sections[accountBalanceIndex]);

            return new Transaction(dateTime, description, value, accountBalance, String.Empty, "Unknown");
        }

        private static ITransaction CreateMoneyVisualizerTransaction(string[] sections)
        {
            const int dateTimeIndex = 0;
            const int vendorIndex = 1;
            const int valueIndex = 2;
            const int accountBalanceIndex = 3;
            const int categoryIndex = 4;
            const int descriptionIndex = 5;

            DateTime dateTime = DateTime.Parse(sections[dateTimeIndex]);
            string vendor = sections[vendorIndex];

            decimal value;
            if (String.IsNullOrWhiteSpace(sections[valueIndex]) ||
                !Decimal.TryParse(sections[valueIndex], out value))
            {
                return NoneTransaction.Instance;
            }

            decimal accountBalance;
            if (String.IsNullOrWhiteSpace(sections[accountBalanceIndex]) ||
                !Decimal.TryParse(sections[accountBalanceIndex], out accountBalance))
            {
                return NoneTransaction.Instance;
            }

            string category = sections[categoryIndex];
            string description = sections[descriptionIndex];

            return new Transaction(dateTime, vendor, value, accountBalance, description, category);
        }
    }
}
