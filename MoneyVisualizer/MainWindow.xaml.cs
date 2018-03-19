using Microsoft.Win32;
using MoneyVisualizer.LineGraph;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MoneyVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
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

            LineGraphViewModel lineGraphViewModel = new LineGraphViewModel(
                transactions,
                new TransactionsFactory(),
                dateAxis,
                balanceAxis);

            plotter.DataContext = lineGraphViewModel;
            plotter.Visibility = Visibility.Visible;
            plotter.LegendVisible = false;
        }
    }
}
