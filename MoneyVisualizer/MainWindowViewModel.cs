using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Win32;
using MoneyVisualizer.LineGraph;
using MoneyVisualizer.Helpers.Ui;

namespace MoneyVisualizer
{
    /// <summary>
    /// The view model for the main window.
    /// </summary>
    public sealed class MainWindowViewModel : NotifyPropertyChanged
    {
        private readonly DateTimeAxis _dateTimeAxis;
        private readonly IntegerAxis _balanceAxis;

        private LineGraphViewModel _lineGraphViewModel;

        /// <summary>
        /// Creates a mew <see cref="MainWindowViewModel"/> object.
        /// </summary>
        /// <param name="dateTimeAxis">The <see cref="DateTimeAxis"/> to use for data conversion so DynamicDataDisplay (D3) can display the dates/times of the transactions.</param>
        /// <param name="balanceAxis">The <see cref="IntegerAxis"/> to use for data conversion so DynamicDataDisplay (D3) can display the balances of the transactions.</param>
        public MainWindowViewModel(DateTimeAxis dateTimeAxis, IntegerAxis balanceAxis)
        {
            LoadTransactionsCommand = new RelayCommand(LoadTransactions);

            _balanceAxis = balanceAxis;
            _dateTimeAxis = dateTimeAxis;
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
                new TransactionsFactory(),
                _dateTimeAxis,
                _balanceAxis);
        }
    }
}
