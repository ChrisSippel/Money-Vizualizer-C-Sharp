using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

using Microsoft.Win32;
using MoneyVisualizer.LineGraph;
using MoneyVisualizer.Helpers.Ui;
using MoneyVisualizer.QuickInfoPage;
using MoneyVisualizer.TransactionsList;

namespace MoneyVisualizer
{
    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public sealed class MainWindowViewModel : NotifyPropertyChanged
    {
        private LineGraphViewModel _lineGraphViewModel;
        private QuickInfoPageViewModel _quickInfoPageViewModel;
        private TransactionsListViewModel _transactionsListViewModel;

        /// <summary>
        /// Creates a mew <see cref="MainWindowViewModel"/> object.
        /// </summary>
        public MainWindowViewModel()
        {
            LoadTransactionsCommand = new RelayCommand(LoadTransactions);
        }

        /// <summary>
        /// The command to call when the user wants to load transactions.
        /// </summary>
        public ICommand LoadTransactionsCommand { get; }

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

            LineGraphViewModel = new LineGraphViewModel(
                transactions,
                new TransactionsFactory());

            QuickInfoPageViewModel = new QuickInfoPageViewModel(
                transactions,
                new TransactionsFactory());

            TransactionsListViewModel = new TransactionsListViewModel(
                transactions,
                new TransactionsFactory());
        }
    }
}
