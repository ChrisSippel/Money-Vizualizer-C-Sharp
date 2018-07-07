using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyVisualizer
{
    public interface ITransactionsManager
    {
        /// <summary>
        /// Creates a collection of <see cref="ITransaction"/> objects from the given file path.
        /// </summary>
        /// <param name="filePath">The path to the file to create the transactions from.</param>
        /// <param name="transactionsType">The type of transactions being loaded.</param>
        /// <returns>A collection of <see cref="ITransaction"/> objects made from the given file.</returns>
        IEnumerable<ITransaction> CreateTransactionsFromFile(string filePath, SupportedTransactionTypes transactionsType);

        /// <summary>
        /// Replaces any <see cref="ITransaction"/> objects in the given collection of <see cref="ITransaction"/> objects, with <see cref="ITransaction"/> objects
        /// from the given file, if the <see cref="ITransaction"/> in the given collection matches the given <see cref="Func{T, TResult}"/>.
        /// </summary>
        /// <param name="toReplaceFunc">The <see cref="Func{T, TResult}"/> that determines which <see cref="ITransaction"/> objects to replace.</param>
        /// <param name="filePath">The path to the file to load the new <see cref="ITransaction"/> objects from.</param>
        /// <param name="transactions">The collection of <see cref="ITransaction"/> objects to potential replace.</param>
        void ReplaceWithTransactionsFromFile(
            Func<ITransaction, bool> toReplaceFunc,
            string filePath,
            ObservableCollection<ITransaction> transactions,
            SupportedTransactionTypes transactionsType);
    }
}
