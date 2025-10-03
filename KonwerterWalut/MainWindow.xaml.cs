using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KonwerterWalut;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly double EUR_RATE = 4.3205;
    private readonly double USD_RATE = 3.9847;
    private readonly double GBP_RATE = 5.1523;

    public MainWindow()
    {
        InitializeComponent();
        btnConvert.Click += BtnConvert_Click;
        btnClear.Click += BtnClear_Click;
    }

    private void BtnConvert_Click(object sender, RoutedEventArgs e)
    {
        string source = ((ComboBoxItem)comboSourceCurrency.SelectedItem).Tag.ToString();
        string target = ((ComboBoxItem)comboTargetCurrency.SelectedItem).Tag.ToString();

        if (!double.TryParse(txtAmount.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
        {
            MessageBox.Show("Wprowadź poprawną kwotę (cyfry i kropka)", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (amount < 0.01 || amount > 999999.99)
        {
            MessageBox.Show("Kwota musi być w zakresie 0.01 - 999,999.99", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (source == target)
        {
            MessageBox.Show("Waluta źródłowa musi być różna od docelowej", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        double fee = Math.Round(amount * 0.02, 2);
        double netAmount = amount - fee;

        double amountPLN = source switch
        {
            "PLN" => netAmount,
            "EUR" => netAmount * EUR_RATE,
            "USD" => netAmount * USD_RATE,
            "GBP" => netAmount * GBP_RATE,
            _ => netAmount
        };

        double result = target switch
        {
            "PLN" => amountPLN,
            "EUR" => amountPLN / EUR_RATE,
            "USD" => amountPLN / USD_RATE,
            "GBP" => amountPLN / GBP_RATE,
            _ => amountPLN
        };
        result = Math.Round(result, 2);

        double rate = 1.0;
        if (source == "PLN")
        {
            rate = target switch
            {
                "EUR" => 1 / EUR_RATE,
                "USD" => 1 / USD_RATE,
                "GBP" => 1 / GBP_RATE,
                _ => 1
            };
        }
        else if (target == "PLN")
        {
            rate = source switch
            {
                "EUR" => EUR_RATE,
                "USD" => USD_RATE,
                "GBP" => GBP_RATE,
                _ => 1
            };
        }
        else
        {
            double sourceToPLN = source switch
            {
                "EUR" => EUR_RATE,
                "USD" => USD_RATE,
                "GBP" => GBP_RATE,
                _ => 1
            };
            double PLNToTarget = target switch
            {
                "EUR" => 1 / EUR_RATE,
                "USD" => 1 / USD_RATE,
                "GBP" => 1 / GBP_RATE,
                _ => 1
            };
            rate = sourceToPLN * PLNToTarget;
        }

        lblResult.Text = $"Wynik konwersji: {result} {target}";
        lblFee.Text = $"Opłata za wymianę: {fee} {source}";
        lblRate.Text = $"Kurs wymiany: 1 {target} = {Math.Round(rate, 4)} {source}";
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        comboSourceCurrency.SelectedIndex = 0;
        comboTargetCurrency.SelectedIndex = 1;
        txtAmount.Text = "";
        lblResult.Text = "Wynik konwersji: ";
        lblFee.Text = "Opłata za wymianę: ";
        lblRate.Text = "Kurs wymiany: ";
    }
}