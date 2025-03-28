using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator
{
    public partial class ProgrammerWindow : Window
    {
        private ProgCalc progCalc;
        public ProgrammerWindow()
        {
            InitializeComponent();
            progCalc = new ProgCalc();
            this.DataContext = progCalc;
            this.ResizeMode = ResizeMode.NoResize;
        }
        private void DefaultMode_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        private void FileCut_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(progCalc.Number2);
            progCalc.Number2 = "0";
            progCalc.RaisePropertyChanged(nameof(progCalc.DisplayNumber));
        }
        private void FileCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(progCalc.Number2);
        }
        private void FilePaste_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string textFromClipboard = Clipboard.GetText();
                if (BigInteger.TryParse(textFromClipboard, out BigInteger result))
                {
                    progCalc.Number2 = result.ToString();
                    progCalc.RaisePropertyChanged(nameof(progCalc.DisplayNumber));
                }
                else
                {
                    MessageBox.Show("Conținutul clipboard nu reprezintă un număr valid!",
                                    "Eroare Paste", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Clipboard-ul nu conține text!",
                                "Eroare Paste", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Programmer Mode", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void BaseSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string newBase = btn.Tag.ToString();
                progCalc.SelectedBase = newBase;
                UpdateButtonsForBase(newBase);
            }
        }
        private void UpdateButtonsForBase(string baseName)
        {
            UpdateButtonsForBaseRecursive(this, baseName);
        }
        private void UpdateButtonsForBaseRecursive(DependencyObject parent, string baseName)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Button btn)
                {
                    if (btn.Tag != null && (btn.Tag.ToString() == "DEC" ||
                                            btn.Tag.ToString() == "HEX" ||
                                            btn.Tag.ToString() == "OCT" ||
                                            btn.Tag.ToString() == "BIN"))
                    {
                    }
                    else
                    {
                        string content = btn.Content.ToString().ToUpper();
                        if (content == "C")
                        {
                            btn.IsEnabled = true;
                        }
                        else if (int.TryParse(content, out int digit))
                        {
                            switch (baseName)
                            {
                                case "BIN":
                                    btn.IsEnabled = (digit == 0 || digit == 1);
                                    break;
                                case "OCT":
                                    btn.IsEnabled = (digit >= 0 && digit <= 7);
                                    break;
                                case "DEC":
                                    btn.IsEnabled = (digit >= 0 && digit <= 9);
                                    break;
                                case "HEX":
                                    btn.IsEnabled = true;
                                    break;
                                default:
                                    btn.IsEnabled = true;
                                    break;
                            }
                        }
                        else
                        {
                            if ("ABCDEF".Contains(content))
                            {
                                btn.IsEnabled = (baseName == "HEX");
                            }
                            else if (content == ".")
                            {
                                btn.IsEnabled = (baseName == "DEC");
                            }
                        }
                    }
                }
                UpdateButtonsForBaseRecursive(child, baseName);
            }
        }
        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string digitStr = btn.Content.ToString();
                if (int.TryParse(digitStr, out int digit))
                {
                    progCalc.AddDigit(digit);
                }
            }
        }
        private void DecimalPoint_Click(object sender, RoutedEventArgs e)
        {
            if (progCalc.SelectedBase == "DEC")
                progCalc.MakePoint();
        }
        private void Sign_Click(object sender, RoutedEventArgs e)
        {
            progCalc.ChangeSign();
        }
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string op = btn.Content.ToString();
                progCalc.AddOperator(op);
            }
        }
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            progCalc.Equals();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            progCalc.C();
        }
        private void CE_Click(object sender, RoutedEventArgs e)
        {
            progCalc.CE();
        }
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            progCalc.Backspace();
        }
        private void Fraction_Click(object sender, RoutedEventArgs e)
        {
            progCalc.Fraction();
        }
        private void Sqr_Click(object sender, RoutedEventArgs e)
        {
            progCalc.Sqr();
        }
        private void Sqrt_Click(object sender, RoutedEventArgs e)
        {
            progCalc.Sqrt();
        }
        private void HexDigit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string hexChar = btn.Content.ToString();
                if (hexChar.Length == 1)
                    progCalc.AddHexChar(hexChar[0]);
            }
        }
        private void Memory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string content = btn.Content.ToString();
                switch (content)
                {
                    case "MC":
                        progCalc.MC();
                        break;
                    case "MR":
                        progCalc.MR();
                        break;
                    case "M+":
                        progCalc.MPlus();
                        break;
                    case "M-":
                        progCalc.MMinus();
                        break;
                    case "MS":
                        progCalc.MS();
                        break;
                }
            }
        }
    }
}
