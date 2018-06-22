using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;
using MoneyVisualizer.Helpers;
using MoneyVisualizer.LineGraph;
using MoneyVisualizer.Helpers.Ui;
using MoneyVisualizer.PieChart;
using MoneyVisualizer.QuickInfoPage;
using MoneyVisualizer.TransactionsList;
using MvvmDialogs;

namespace MoneyVisualizer
{
    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public sealed class MainWindowViewModel : NotifyPropertyChanged
    {
        private readonly IDialogService _dialogService;
        private LineGraphViewModel _lineGraphViewModel;
        private QuickInfoPageViewModel _quickInfoPageViewModel;
        private TransactionsListViewModel _transactionsListViewModel;
        private PieChartViewModel _pieChartViewModel;
        private IReadOnlyList<ITransaction> _transactions;

        /// <summary>
        /// Creates a mew <see cref="MainWindowViewModel"/> object.
        /// </summary>
        public MainWindowViewModel(IDialogService dialogService)
        {
            LoadTransactionsCommand = new RelayCommand(LoadTransactions);
            SaveTransactionsCommand = new RelayCommand(SaveTransactions);

            _dialogService = dialogService;
        }

        /// <summary>
        /// The command to call when the user wants to load transactions.
        /// </summary>
        public ICommand LoadTransactionsCommand { get; }

        /// <summary>
        /// The command to call when the user wants to load transactions.
        /// </summary>
        public ICommand SaveTransactionsCommand { get; }

        /// <summary>
        /// The collection of transactions that have been loaded.
        /// </summary>
        public IReadOnlyList<ITransaction> Transactions
        {
            get
            {
                return _transactions;
            }

            set
            {
                if (_transactions != null &&
                    _transactions.Equals(value))
                {
                    return;
                }

                _transactions = value;
            }
        }

        /// <summary>
        /// The view model for the displaying of a line graph of the loaded transactions.
        /// </summary>
        public LineGraphViewModel LineGraphViewModel
        {
            get
            {
                return _lineGraphViewModel;
            }

            set
            {
                if (_lineGraphViewModel == value)
                {
                    return;
                }

                _lineGraphViewModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The view model for the displaying of a summary of information for the loaded transactions.
        /// </summary>
        public QuickInfoPageViewModel QuickInfoPageViewModel
        {
            get
            {
                return _quickInfoPageViewModel;
            }

            set
            {
                if (_quickInfoPageViewModel == value)
                {
                    return;
                }

                _quickInfoPageViewModel = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The view model for the displaying of all of the loaded transactions.
        /// </summary>
        public TransactionsListViewModel TransactionsListViewModel
        {
            get
            {
                return _transactionsListViewModel;
            }

            set
            {
                if (_transactionsListViewModel == value)
                {
                    return;
                }

                _transactionsListViewModel = value;
                OnPropertyChanged();
            }
        }

        public PieChartViewModel PieChartViewModel
        {
            get
            {
                return _pieChartViewModel;
            }

            set
            {
                if (_pieChartViewModel == value)
                {
                    return;
                }

                _pieChartViewModel = value;
                OnPropertyChanged();
            }
        }

        private void LoadTransactions(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
            };

            bool? result = openFileDialog.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                return;
            }

            List<string> transactions = new List<string>();
            using (StreamReader reader = new StreamReader(openFileDialog.FileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    transactions.Add(line);
                }
            }

            var transactionsFactory = new TransactionsFactory();
            var transactionsList = new List<ITransaction>();
            foreach (var transaction in transactions)
            {
                var createdTransaction = transactionsFactory.CreateTransaction(transaction);
                if (createdTransaction != NoneTransaction.Instance)
                {
                    transactionsList.Add(createdTransaction);
                }
            }

            Transactions = transactionsList;

            LineGraphViewModel = new LineGraphViewModel(Transactions);
            QuickInfoPageViewModel = new QuickInfoPageViewModel(Transactions);
            TransactionsListViewModel = new TransactionsListViewModel(Transactions);
            PieChartViewModel = new PieChartViewModel(Transactions);
        }

        private void SaveTransactions(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV Files| *.csv";
            bool? saveFileResult = saveFileDialog.ShowDialog();
            if (!saveFileResult.HasValue || !saveFileResult.Value)
            {
                return;
            }

            using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                foreach (var transaction in Transactions)
                {
                    writer.WriteLine($"{transaction.DateTime.ToString("MM'/'dd'/'yyyy")},{transaction.Vendor},{transaction.Value},{transaction.AccountBalance},{transaction.Category},{transaction.Description}");
                }
            }

            _dialogService.ShowMessageBox(this, "File saved", "File saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
