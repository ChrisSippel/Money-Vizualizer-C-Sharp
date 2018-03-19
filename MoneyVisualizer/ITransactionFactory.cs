using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyVisualizer
{
    public interface ITransactionFactory
    {
        ITransaction CreateDebitTransaction(string transaction);

        ITransaction CreateCreditTransaction(string transaction);
    }
}
