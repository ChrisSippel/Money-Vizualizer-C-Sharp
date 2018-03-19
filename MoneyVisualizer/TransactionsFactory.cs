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
        public ITransaction CreateDebitTransaction(string transaction)
        {
            const int dateTimeIndex = 0;
            const int descriptionIndex = 1;
            const int costValueIndex = 2;
            const int creditValueIndex = 3;
            const int accountBalanceIndex = 4;

            if (string.IsNullOrWhiteSpace(transaction))
            {
                throw new ArgumentNullException(nameof(transaction), $"!string.IsNullOrWhiteSpace({nameof(transaction)})");
            }

            string[] sections = transaction.Split(',');
            if (sections.Length < 4)
            {
                return NoneTransaction.Instance;
            }

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

            return new DebitTransaction(dateTime, description, value, accountBalance);
        }

        public ITransaction CreateCreditTransaction(string transaction)
        {
            return NoneTransaction.Instance;
        }
    }
}
