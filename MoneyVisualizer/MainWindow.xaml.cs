﻿using System.Windows;
using MvvmDialogs;

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
            DataContext = new MainWindowViewModel(
                new DialogService(),
                new TransactionsManager(new TransactionsFactory()));
        }
    }
}
