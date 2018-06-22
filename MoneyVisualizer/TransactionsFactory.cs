using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyVisualizer
{
    public sealed class TransactionsFactory : ITransactionFactory
    {
        public ITransaction CreateTransaction(string transaction)
        {
            if (string.IsNullOrWhiteSpace(transaction))
            {
                throw new ArgumentNullException(nameof(transaction), $"!string.IsNullOrWhiteSpace({nameof(transaction)})");
            }

            string[] sections = transaction.Split(',');
            if (sections.Length == 5)
            {
                return CreateBankTransaction(sections);
            }

            if (sections.Length == 6)
            {
                return CreateMoneyVisualizerTransaction(sections);
            }

            return NoneTransaction.Instance;
        }

        private static ITransaction CreateBankTransaction(string[] sections)
        {
            const int dateTimeIndex = 0;
            const int descriptionIndex = 1;
            const int costValueIndex = 2;
            const int creditValueIndex = 3;
            const int accountBalanceIndex = 4;

            DateTime dateTime = DateTime.ParseExact(sections[dateTimeIndex], "MM/dd/yyyy", CultureInfo.InvariantCulture);
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

            return new Transaction(dateTime, description, value, accountBalance, string.Empty, "Unknown");
        }

        private static ITransaction CreateMoneyVisualizerTransaction(string[] sections)
        {
            const int dateTimeIndex = 0;
            const int vendorIndex = 1;
            const int valueIndex = 2;
            const int accountBalanceIndex = 3;
            const int categoryIndex = 4;
            const int descriptionIndex = 5;

            DateTime dateTime = DateTime.ParseExact(sections[dateTimeIndex], "MM'/'dd'/'yyyy", CultureInfo.InvariantCulture);
            string vendor = sections[vendorIndex];

            decimal value;
            if (string.IsNullOrWhiteSpace(sections[valueIndex]) ||
                !decimal.TryParse(sections[valueIndex], out value))
            {
                return NoneTransaction.Instance;
            }

            decimal accountBalance;
            if (string.IsNullOrWhiteSpace(sections[accountBalanceIndex]) ||
                !decimal.TryParse(sections[accountBalanceIndex], out accountBalance))
            {
                return NoneTransaction.Instance;
            }

            string category = sections[categoryIndex];
            string description = sections[descriptionIndex];

            return new Transaction(dateTime, vendor, value, accountBalance, description, category);
        }
    }
}
