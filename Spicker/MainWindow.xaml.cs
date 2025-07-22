using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Spicker.DataProviders;
using Spicker.Engine;
using Spicker.Engines.RuleSet;
using Spicker.Rules;

namespace Spicker
{
    public partial class MainWindow : Window
    {
        // UI elements
        private const string WindowTitle = "Spicker - Stock Analysis Tool";
        private const int WindowWidth = 600;
        private const int WindowHeight = 400;
        // UI components

        private ComboBox stockComboBox;
        private TextBlock deepDiveResultText;
        private TextBlock watchResultText;

        // Add service fields
        private readonly StockService stockService = new StockService();
        private readonly StockDeepDiveService deepDiveService = new StockDeepDiveService();
        private readonly StockWatchService watchService = new StockWatchService();
        private readonly IExtendedHistoricalDataProvider historicalProvider = new LeanHistoricalDataProvider();
        private readonly List<IRule> rules = new List<IRule>
        {
            new SimpleMomentumRule()
        };

        private readonly RuleSuiteEngine ruleEngine;


        public MainWindow()
        {
            InitializeComponent();
            ruleEngine = new RuleSuiteEngine(historicalProvider, rules);
            SetupUI();
        }

        private void SetupUI()
        {
            // Main vertical stack
            var mainStack = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(20) };

            // Section 1: Get Stocks
            var section1 = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            var getStocksButton = new Button { Content = "Get Stocks", Width = 100, Margin = new Thickness(0, 0, 10, 0) };
            stockComboBox = new ComboBox { Width = 100 };
            section1.Children.Add(getStocksButton);
            section1.Children.Add(stockComboBox);
            mainStack.Children.Add(section1);

            // Section 2: Deep Dive Stock
            var section2 = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            var deepDiveButton = new Button { Content = "Deep Dive Stock", Width = 120, Margin = new Thickness(0, 0, 10, 0) };
            deepDiveResultText = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
            section2.Children.Add(deepDiveButton);
            section2.Children.Add(deepDiveResultText);
            mainStack.Children.Add(section2);

            // Section 3: Watch <value>
            var section3 = new StackPanel { Orientation = Orientation.Horizontal };
            var watchButton = new Button { Content = "Watch", Width = 100, Margin = new Thickness(0, 0, 10, 0) };
            watchResultText = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
            section3.Children.Add(watchButton);
            section3.Children.Add(watchResultText);
            mainStack.Children.Add(section3);

            this.Content = mainStack;

            // Event handlers
            getStocksButton.Click += GetStocksButton_Click;
            deepDiveButton.Click += DeepDiveButton_Click;
            watchButton.Click += WatchButton_Click;
        }

        private void GetStocksButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                var stocks = stockService.GetStocks();
                Dispatcher.Invoke(() =>
                {
                    stockComboBox.ItemsSource = stocks;
                    if (stocks.Count > 0)
                        stockComboBox.SelectedIndex = 0;
                });
            });
        }

        private async void DeepDiveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStock = stockComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedStock))
                return;

            deepDiveResultText.Text = "Analyzing...";

            try
            {
                var results = await ruleEngine.AnalyzeAsync(new List<string> { selectedStock });
                if (results.TryGetValue(selectedStock, out var analysis))
                {
                    var summary = $"Score: {analysis.Score}\nDirection: {analysis.Direction}\nReasons:\n- {string.Join("\n- ", analysis.Reasons)}";
                    Dispatcher.Invoke(() => deepDiveResultText.Text = summary);
                }
                else
                {
                    Dispatcher.Invoke(() => deepDiveResultText.Text = "No result.");
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => deepDiveResultText.Text = $"Error: {ex.Message}");
            }
        }


        private void WatchButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStock = stockComboBox.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedStock))
                return;

            Task.Run(() =>
            {
                var result = watchService.Watch(selectedStock);
                Dispatcher.Invoke(() =>
                {
                    watchResultText.Text = result;
                });
            });
        }
    }
}