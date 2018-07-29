using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        private readonly ITransactionsManager _transactionsManager;
        private LineGraphViewModel _lineGraphViewModel;
        private QuickInfoPageViewModel _quickInfoPageViewModel;
        private TransactionsListViewModel _transactionsListViewModel;
        private PieChartViewModel _pieChartViewModel;
        private ObservableCollection<ITransaction> _transactions;

        /// <summary>
        /// Creates a mew <see cref="MainWindowViewModel"/> object.
        /// </summary>
        public MainWindowViewModel(IDialogService dialogService, ITransactionsManager transactionsManager)
        {
            LoadTdBankAccountTransactionsCommand = new RelayCommand(LoadTdBankAccountTransactions);
            LoadMoneyVisualizerTransactionsCommand = new RelayCommand(LoadMoneyVisualizerTransactions);
            LoadTdCreditCardTransactionsCommand = new RelayCommand(LoadTdCreditCardTransactions);
            LoadCapitalOneCreditCardTransactionsCommand = new RelayCommand(LoadCapitalOneCreditCardTransactions);

            SaveTransactionsCommand = new RelayCommand(SaveTransactions);

            _dialogService = dialogService;
            _transactionsManager = transactionsManager;
        }

        /// <summary>
        /// The command to call when the user wants to load transactions from a TD bank account.
        /// </summary>
        public ICommand LoadTdBankAccountTransactionsCommand { get; }

        /// <summary>
        /// The command to call when the user wants to load transactions saved from Money Visualizer.
        /// </summary>
        public ICommand LoadMoneyVisualizerTransactionsCommand { get; }

        /// <summary>
        /// The command to call when the user wants to load transactions from a TD credit card.
        /// </summary>
        public ICommand LoadTdCreditCardTransactionsCommand { get; }

        /// <summary>
        /// The command to call when the user wants to load transactions from a Capital One credit card.
        /// </summary>
        public ICommand LoadCapitalOneCreditCardTransactionsCommand { get; }

        /// <summary>
        /// The command to call when the user wants to load transactions.
        /// </summary>
        public ICommand SaveTransactionsCommand { get; }

        /// <summary>
        /// The collection of transactions that have been loaded.
        /// </summary>
        public ObservableCollection<ITransaction> Transactions
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

        /// <summary>
        /// The view model for displaying all of the categories of the transactions
        /// and how much money is assigned to each category.
        /// </summary>
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

        private void LoadTdBankAccountTransactions(object obj)
        {
            LoadTransactions(SupportedTransactionTypes.TdBankAccount);
        }

        private void LoadTdCreditCardTransactions(object obj)
        {
            Func<ITransaction, bool> toReplaceFunc = transaction => transaction.Category == "Credit Card";

            ReplaceTransactions(toReplaceFunc, SupportedTransactionTypes.TdCreditCard);
        }

        private void LoadMoneyVisualizerTransactions(object obj)
        {
            LoadTransactions(SupportedTransactionTypes.MoneyVisualizer);
        }

        private void LoadCapitalOneCreditCardTransactions(object obj)
        {
            Func<ITransaction, bool> toReplaceFunc = transaction => transaction.Category == "Credit Card";

            ReplaceTransactions(toReplaceFunc, SupportedTransactionTypes.CapitalOneCreditCard);
        }

        private void LoadTransactions(SupportedTransactionTypes transactionsType)
        {
            var transactionsFilePath = GetTransactionsFilePath();
            if (string.IsNullOrWhiteSpace(transactionsFilePath))
            {
                return;
            }

            var transactions = _transactionsManager.CreateTransactionsFromFile(transactionsFilePath, transactionsType);
            var observableTransactions = new ObservableCollection<ITransaction>(transactions);

            Transactions = observableTransactions;

            LineGraphViewModel = new LineGraphViewModel(Transactions);
            QuickInfoPageViewModel = new QuickInfoPageViewModel(Transactions);
            TransactionsListViewModel = new TransactionsListViewModel(Transactions);
            PieChartViewModel = new PieChartViewModel(Transactions);
        }

        private void ReplaceTransactions(Func<ITransaction, bool> toReplaceFunc, SupportedTransactionTypes supportedTransactionType)
        {
            string transactionsFile = GetTransactionsFilePath();
            _transactionsManager.ReplaceWithTransactionsFromFile(
                toReplaceFunc,
                transactionsFile,
                _transactions,
                supportedTransactionType);
        }

        private string GetTransactionsFilePath()
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
                return string.Empty;
            }

            return openFileDialog.FileName;
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
