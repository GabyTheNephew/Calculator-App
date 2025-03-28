using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Buffers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Calculator
{
    internal class Calc:INotifyPropertyChanged
    {
        private bool _areButtonsEnabled = true;

        private bool _concatenateOperand = true;
        private bool _concatenateToNumber2 = true;
        private bool _isMButtonEnabled = false;
        private bool _isDigitGroupingEnabled = false;

        private string _number = "0";
        private string _operator = "";
        private string _number2 = "0";
        private string _operation = "";
        
        private string _selectedMemory;
        private ObservableCollection<double> _memoryEntries = new ObservableCollection<double>();

        public bool IsDigitGroupingEnabled
        {
            get => _isDigitGroupingEnabled;
            set
            {
                _isDigitGroupingEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsMButtonEnabled
        {
            get => _isMButtonEnabled;
            set
            {
                _isMButtonEnabled=value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<double> MemoryEntries
        {
            get => _memoryEntries;
            set
            {
                _memoryEntries = value;
                OnPropertyChanged();
            }
        }

        public string SelectedMemory
        {
            get => _selectedMemory;
            set
            {
                _selectedMemory = value;
                OnPropertyChanged();
                if (_selectedMemory != null)
                {
                    Number2 = _selectedMemory.ToString();
                }
            }
        }
        public bool ConcatenateToNumber2
        {
            get => _concatenateToNumber2;
            set
            {
                _concatenateToNumber2 = value;
                OnPropertyChanged();
            }
        }
        public bool AreButtonsEnabled
        {
            get => _areButtonsEnabled;
            set
            {
                if (_areButtonsEnabled != value)
                {
                    _areButtonsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Operation
        {
            get => _operation;
            set
            {
                _operation = value;
                OnPropertyChanged();
            }
        }
        public string Number2
        {
            get => _number2;
            set
            {
                _number2 = value;
                OnPropertyChanged();
            }
        }
        public string Number
        {
            get => _number;
            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Operator
        {
            get => _operator;
            set
            {
                if (_operator != value)
                {
                    _operator = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public void ChangeSign()
        {
            double.TryParse(Number2, out double nr);
            nr*= (-1);
            Number2 = nr.ToString();
        }
        public void MakePoint()
        {
            if(Number2.Contains("."))
            {
                return;
            }
            Number2 += ".";
        }

        public void ProcessKey(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
            {
                int digit = key - Key.D0;
                AddDigit(digit);
                return;
            }

            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                int digit = key - Key.NumPad0;
                AddDigit(digit);
                return;
            }

            if (key == Key.Add)
            {
                AddOperator("+");
                return;
            }
            else if (key == Key.Subtract)
            {
                AddOperator("-");
                return;
            }
            else if (key == Key.Multiply)
            {
                AddOperator("x");
                return;
            }
            else if (key == Key.Divide)
            {
                AddOperator("/");
                return;
            }

            if (key == Key.Oem2)
            {
                AddOperator("/");
                return;
            }
            if (key == Key.OemMinus)
            {
                AddOperator("-");
                return;
            }
            if (key == Key.OemPlus)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    AddOperator("+");
                    return;
                }
            }

            if (key == Key.Enter)
            {
                Equals();
                return;
            }

            if (key == Key.Escape)
            {
                C();
                return;
            }

            if (key == Key.OemComma || key == Key.Decimal || key == Key.OemPeriod)
            {
                MakePoint();
                return;
            }

            if (key == Key.Back)
            {
                Backspace();
                return;
            }
        }


        private string Punctuate(string input)
        {
            if (!IsDigitGroupingEnabled)
            {
                return input;
            }

            var culture = System.Globalization.CultureInfo.CurrentCulture;

            if (long.TryParse(input, NumberStyles.AllowThousands, culture, out long number))
            {
                return number.ToString("N0", culture);
            }
            else
            {
                return input;
            }
        }


        public void MS()
        {
            double.TryParse(Number2, out double nr);
            MemoryEntries.Add(nr);
            IsMButtonEnabled = true;
        }
        public void MR()
        {
            if(MemoryEntries.Count > 0)
            {
                Number2 = MemoryEntries[MemoryEntries.Count - 1].ToString();
            }
        }
        public void MC()
        {
            MemoryEntries.Clear();
            IsMButtonEnabled = false;
        }
        public void MMinus()
        {
            if (double.TryParse(Number2, out double nr))
            {
                if (MemoryEntries.Count == 0)
                {
                    MemoryEntries.Add(nr*(-1));
                }
                else
                {
                    MemoryEntries[MemoryEntries.Count - 1] -= nr;
                    Console.WriteLine(MemoryEntries[MemoryEntries.Count - 1]);
                }
                IsMButtonEnabled = true;
            }
            
        }
        public void MPlus()
        {
            if (double.TryParse(Number2, out double nr))
            {
                if (MemoryEntries.Count == 0)
                {
                    MemoryEntries.Add(nr);
                }
                else
                {
                    MemoryEntries[MemoryEntries.Count - 1] += nr;
                    Console.WriteLine(MemoryEntries[MemoryEntries.Count - 1]);
                }
                IsMButtonEnabled = true;
            }
        }
        public void Equals()
        {
            if(!AreButtonsEnabled)
            {
                C();
            }
            if(string.IsNullOrEmpty(Operator))
            {
                return;
            }
            if(Operation.Contains("="))
            {
                return;
            }
            if (_concatenateOperand)
                Operation += Number2;

            double.TryParse(Number, out double nr1);
            double.TryParse(Number2, out double nr2);
            if (Operator == "+")
            {
                Number2 = (nr1 + nr2).ToString();
                Number = "";
            }
            else if (Operator=="-")
            {
                Number2 = (nr1 - nr2).ToString();
                Number = "";
            }
            else if (Operator=="x")
            {
                Number2 = (nr1 * nr2).ToString();
                Number = "";
            }
            else if (Operator=="/")
            {
                if(nr2==0)
                {
                    Number2 = "Cannot divide by 0";
                    AreButtonsEnabled = false;
                }
                else
                {
                    Number2 = (nr1 / nr2).ToString();
                    Number = "";
                }
            }
            _concatenateOperand = true;
            Operation += "=";
        }
        public void Percent()
        {
            if (Number != "0")
            {
                double.TryParse(Number2, out double nr);
                Operation = Number + Operator + $"Sqr({Number2})";
                _concatenateOperand = false;
                Number2 = (nr/100).ToString();
            }
            else
            {
                double.TryParse(Number2, out double nr);
                Operation = $"Sqr({Number2})";
                _concatenateOperand = false;
                Number2 = (nr/100).ToString();
            }
        }
        public void ComputeOperation()
        {
            double.TryParse(Number, out double nr1);
            double.TryParse(Number2, out double nr2);
            double result = 0;

            switch (Operator)
            {
                case "+":
                    result = nr1 + nr2;
                    break;
                case "-":
                    result = nr1 - nr2;
                    break;
                case "x":
                    result = nr1 * nr2;
                    break;
                case "/":
                    if (nr2 == 0)
                    {
                        Number2 = "Cannot divide by 0";
                        AreButtonsEnabled = false;
                        return;
                    }
                    result = nr1 / nr2;
                    break;
            }

            Number2 = result.ToString();
            Number = Number2;
            ConcatenateToNumber2 = false;
        }

        public void AddOperator(string input)
        {
            if (Operation.Contains("="))
            {
                Operation = Number2;
                Number = "0";
                Operator = "";
            }
            else if (!string.IsNullOrEmpty(Operator))
            {
                ComputeOperation();
            }

            Operator = input;
            Operation = Number2 + input;
        }

        public void Fraction()
        {
            if (Operator == "")
            {
                double.TryParse(Number2, out double nr);
                if(nr==0)
                {
                    Number2 = "Cannot divide by zero!";
                    Number = "0";
                    Operation = "";
                    AreButtonsEnabled = false;
                }
                else
                {
                    _concatenateOperand = false;
                    Operation = $"1/({Number2})";
                    Number2 = (1/nr).ToString();
                }
            }
            else
            {
                double.TryParse(Number, out double nr);
                if (nr == 0)
                {
                    Number2 = "Cannot divide by zero!";
                    AreButtonsEnabled = false;
                    Number = "0";
                    Operation = "";
                }
                else
                {
                    Operation = Number + Operator + $"1/({Number2})";
                    _concatenateOperand= false;
                    Number2 = (1/nr).ToString();
                }
            }
        }
        public void Backspace()
        {
            if(Number2=="0")
            {
                Number2= "0";
            }
            else
            {
                if(Number2.Length>1)
                {
                    Number2=Number2.Substring(0, Number2.Length-1);
                }
                else if(Number2.Length==1)
                {
                    Number2= "0";
                }

            }
        }
        public void Sqr()
        {
            if (Number != "0")
            {
                double.TryParse(Number2, out double nr);
                Operation = Number + Operator + $"Sqr({Number2})";
                _concatenateOperand=false;
                Number2 = Math.Pow(nr, 2).ToString();
            }
            else
            {
                double.TryParse(Number2, out double nr);
                Operation = $"Sqr({Number2})";
                _concatenateOperand=false;
                Number2 = Math.Pow(nr, 2).ToString();
            }
        }
        public void Sqrt()
        {
            if(Operator == "")
            {
                double.TryParse(Number2, out double nr);
                Operation = $"√{Number2}";
                _concatenateOperand = false;
                Number2 = Math.Sqrt(nr).ToString();
            }
            else
            {
                double.TryParse(Number2, out double nr);
                Operation = Number + Operator + $"√{Number2}";
                _concatenateOperand=false;
                Number2 = Math.Sqrt(nr).ToString();
            }
        }
        public void CE()
        {
            if(Operation.Contains('='))
            {
                C();
            }
            else
            {
                Number2 = "0";
            }
        }
        public void C()
        {
            Number = "0";
            Number2 = "0";
            Operation = "";
            Operator = "";
            _concatenateOperand = true;
            AreButtonsEnabled = true;
        }
        public void AddDigit(int digit) 
        {
            if(Operation.Contains("="))
            {
                C();
            }

            if(Operator=="")
            {
                if ((Number2 == "0" || Number2 == "Cannot divide by zero!") && digit == 0)
                {
                    Number2 = "0";
                    return;
                }
                else
                {
                    if (Number2 == "0" || Number2 == "Cannot divide by zero!")
                    {
                        Number2 = digit.ToString();
                        return;
                    }

                    Number2 += digit.ToString();
                    if (Number2.Length > 3)
                    {
                        Number2 = Punctuate(Number2);
                    }
                }
            }
            else
            {
                if (Number == "0" && digit == 0)
                {
                    Number = Number2;
                    Number2 = "0";
                    return;
                }
                else
                {
                    if (Number == "0")
                    {
                        Number = Number2;
                        Number2 = digit.ToString();
                        return;
                    }
                    else
                    {
                        if(Number2 == "0")
                        {
                            Number2 = digit.ToString();
                        }
                        else
                        {
                            if (ConcatenateToNumber2)
                            {
                                Number2 += digit.ToString();
                            }
                            else
                            {
                                Number2 = digit.ToString();
                                ConcatenateToNumber2 = true;
                            }
                        }
                    }
                    
                }
            }

        }
    }
}
