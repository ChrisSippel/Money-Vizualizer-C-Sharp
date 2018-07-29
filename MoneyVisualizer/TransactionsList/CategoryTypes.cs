using System.Collections.Generic;

namespace MoneyVisualizer.TransactionsList
{
    public static class CategoryTypes
    {
        public static IReadOnlyCollection<string> List { get; } = new List<string>
        {
            "Unknown",
            "Alcohol",
            "Bills",
            "Charity",
            "Clothing",
            "Credit Card",
            "Car payment",
            "Eating out",
            "Entertainment",
            "Gas",
            "Gift",
            "Groceries",
            "Income",
            "Insurance",
            "Mortgage",
            "Personal Care",
            "Pets",
            "Savings",
            "Shopping",
            "Taxes",
            "Video Games",
        };
    }
}
