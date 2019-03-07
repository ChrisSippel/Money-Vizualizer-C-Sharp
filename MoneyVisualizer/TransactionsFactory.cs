using MoneyVisualizer.TransactionsList;
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

                case SupportedTransactionTypes.TdCreditCard:
                    return CreateTdCreditCardTransaction(sections);

                case SupportedTransactionTypes.MoneyVisualizer:
                    return CreateMoneyVisualizerTransaction(sections);

                case SupportedTransactionTypes.CapitalOneCreditCard:
                    return CreateCaptialOneTransaction(sections);

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

            string category = "Unknown";
            if (description.Trim().Equals("TD MORTGAGE", StringComparison.OrdinalIgnoreCase))
            {
                category = "Mortgage";
            }

            return new Transaction(dateTime, description, value, accountBalance, string.Empty, category);
        }

        private static ITransaction CreateTdCreditCardTransaction(string[] sections)
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

            if (value > 0)
            {
                return NoneTransaction.Instance;
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

        private static ITransaction CreateCaptialOneTransaction(string[] sections)
        {
            const string titleText = "Transaction Date";
            const int purchaseDateIndex = 0;
            const int vendorIndex = 3;
            const int debitIndex = 5;
            const int paymentAndCreditIndex = 6;

            // If we're looking at the title text, or a credit/payment transaction
            // skip it
            if (sections[0] == titleText ||
                !string.IsNullOrWhiteSpace(sections[paymentAndCreditIndex]))
            {
                return NoneTransaction.Instance;
            }

            decimal debit;
            DateTime purchaseDateTime;
            if (!decimal.TryParse(sections[debitIndex], out debit) ||
                !DateTime.TryParse(sections[purchaseDateIndex], out purchaseDateTime))
            {
                return NoneTransaction.Instance;
            }

            debit *= -1;

            return new Transaction(purchaseDateTime, sections[vendorIndex], debit, 0, string.Empty, CategoryTypes.List.GetEnumerator().Current);
        }
    }
}
