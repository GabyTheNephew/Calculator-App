using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace Calculator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var calc = new Calc();
        this.DataContext = calc;
        bool loadedValue = LoadDigitGroupingSetting();
        MenuItemDigitGrouping.IsChecked = loadedValue;
        calc.IsDigitGroupingEnabled = loadedValue;
        this.KeyDown += MainWindow_KeyDown;
        this.ResizeMode = ResizeMode.NoResize;
    }
    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        var calc = this.DataContext as Calc;
        if (calc != null)
        {
            calc.ProcessKey(e.Key);
        }
    }
    private bool LoadDigitGroupingSetting()
    {
        if (!File.Exists("settings.txt"))
            return false;
        try
        {
            string text = File.ReadAllText("settings.txt");
            if (bool.TryParse(text, out bool result))
            {
                return result;
            }
        }
        catch { }
        return false;
    }
    private void SaveDigitGroupingSetting(bool value)
    {
        File.WriteAllText("settings.txt", value.ToString());
    }
    private void Programmer_Click(object sender, RoutedEventArgs e)
    {
        ProgrammerWindow progWin = new ProgrammerWindow();
        progWin.Show();
        this.Close();
    }
    private void FileDigitGrouping_Click(object sender, RoutedEventArgs e)
    {
        var calc = this.DataContext as Calc;
        MenuItem menuItem = sender as MenuItem;
        if (menuItem != null && calc != null)
        {
            bool isChecked = menuItem.IsChecked;
            calc.IsDigitGroupingEnabled = isChecked;
            SaveDigitGroupingSetting(isChecked);
        }
    }
    private void FileCut_Click(object sender, RoutedEventArgs e)
    {
        var calculator = this.DataContext as Calc;
        Clipboard.SetText(calculator.Number2);
        calculator.Number2 = "0";
    }
    private void FileCopy_Click(object sender, RoutedEventArgs e)
    {
        var calculator = this.DataContext as Calc;
        Clipboard.SetText(calculator.Number2);
    }
    private void FilePaste_Click(object sender, RoutedEventArgs e)
    {
        var calculator = this.DataContext as Calc;
        if (Clipboard.ContainsText())
        {
            string textDinClipboard = Clipboard.GetText();
            if (double.TryParse(textDinClipboard, out double rezultat))
            {
                calculator.Number2 = rezultat.ToString();
            }
            else
            {
                MessageBox.Show("Conținutul Clipboard nu este un număr valid!",
                                "Eroare la Paste",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Clipboard-ul nu conține text!",
                            "Eroare la Paste",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
        }
    }
    private void HelpAbout_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("GabyTheNephew Calculator\nAutor: Slav Gabriel-Bogdan\n Grupa 10LF333",
                        "About",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            string input = button.Content.ToString();
            var calculator = this.DataContext as Calc;
            if (int.TryParse(input, out int number))
            {
                calculator.AddDigit(number);
            }
            else if (input == "+")
            {
                calculator.AddOperator(input);
            }
            else if (input == "-")
            {
                calculator.AddOperator(input);
            }
            else if (input == "x")
            {
                calculator.AddOperator(input);
            }
            else if (input == "/")
            {
                calculator.AddOperator(input);
            }
            else if (input == "=")
            {
                calculator.Equals();
            }
            else if (input == "√x")
            {
                calculator.Sqrt();
            }
            else if (input == "x²")
            {
                calculator.Sqr();
            }
            else if (input == "1/x")
            {
                calculator.Fraction();
            }
            else if (input == "C")
            {
                calculator.C();
            }
            else if (input == "CE")
            {
                calculator.CE();
            }
            else if (input == "⌫")
            {
                calculator.Backspace();
            }
            else if (input == "%")
            {
                calculator.Percent();
            }
            else if (input == "±")
            {
                calculator.ChangeSign();
            }
            else if (input == ".")
            {
                calculator.MakePoint();
            }
            else if (input == "M+")
            {
                calculator.MPlus();
            }
            else if (input == "M-")
            {
                calculator.MMinus();
            }
            else if (input == "MC")
            {
                calculator.MC();
            }
            else if (input == "MR")
            {
                calculator.MR();
            }
            else if (input == "MS")
            {
                calculator.MS();
            }
        }
    }
}
