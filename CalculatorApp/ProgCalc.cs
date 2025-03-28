using System;
using System.Globalization;
using System.Numerics;

namespace Calculator
{
    internal class ProgCalc : Calc
    {
        private string _selectedBase = "DEC";
        public string SelectedBase
        {
            get => _selectedBase;
            set
            {
                _selectedBase = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayNumber));
                OnPropertyChanged(nameof(ValueDEC));
                OnPropertyChanged(nameof(ValueHEX));
                OnPropertyChanged(nameof(ValueOCT));
                OnPropertyChanged(nameof(ValueBIN));
            }
        }
        public void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
        public string ValueDEC
        {
            get
            {
                if (BigInteger.TryParse(Number2, out BigInteger dec))
                    return dec.ToString();
                return "Error";
            }
        }
        public string ValueHEX
        {
            get
            {
                if (BigInteger.TryParse(Number2, out BigInteger dec))
                    return dec.ToString("X");
                return "Error";
            }
        }
        public string ValueOCT
        {
            get
            {
                if (BigInteger.TryParse(Number2, out BigInteger dec))
                    return ConvertBigIntegerToBase(dec, 8);
                return "Error";
            }
        }
        public string ValueBIN
        {
            get
            {
                if (BigInteger.TryParse(Number2, out BigInteger dec))
                    return ConvertBigIntegerToBase(dec, 2);
                return "Error";
            }
        }
        public string DisplayNumber
        {
            get
            {
                if (!BigInteger.TryParse(Number2, out BigInteger dec))
                    return "Error";
                if (dec.ToString().Length > 30)
                    return "Overflow";
                switch (SelectedBase)
                {
                    case "DEC":
                        return dec.ToString();
                    case "HEX":
                        return dec.ToString("X");
                    case "OCT":
                        return ConvertBigIntegerToBase(dec, 8);
                    case "BIN":
                        return ConvertBigIntegerToBase(dec, 2);
                    default:
                        return dec.ToString();
                }
            }
        }
        private string ConvertBigIntegerToBase(BigInteger value, int toBase)
        {
            if (value == 0)
                return "0";
            string chars = "0123456789ABCDEF";
            bool isNegative = value < 0;
            if (isNegative)
                value = BigInteger.Negate(value);
            string result = "";
            while (value > 0)
            {
                int remainder = (int)(value % toBase);
                result = chars[remainder] + result;
                value /= toBase;
            }
            return isNegative ? "-" + result : result;
        }
        public new void AddDigit(int digit)
        {
            if (!string.IsNullOrEmpty(Operator) && !ConcatenateToNumber2)
            {
                Number2 = digit.ToString();
                ConcatenateToNumber2 = true;
                OnPropertyChanged(nameof(DisplayNumber));
                OnPropertyChanged(nameof(ValueDEC));
                OnPropertyChanged(nameof(ValueHEX));
                OnPropertyChanged(nameof(ValueOCT));
                OnPropertyChanged(nameof(ValueBIN));
                return;
            }
            if (!IsDigitAllowed(digit))
                return;
            if (Operation.Contains("="))
                C();
            BigInteger currentValue = 0;
            BigInteger.TryParse(Number2, out currentValue);
            int multiplier = SelectedBase switch
            {
                "BIN" => 2,
                "OCT" => 8,
                "DEC" => 10,
                "HEX" => 16,
                _ => 10
            };
            if (Number2 == "0")
                currentValue = 0;
            BigInteger newValue = currentValue * multiplier + digit;
            Number2 = newValue.ToString();
            OnPropertyChanged(nameof(DisplayNumber));
            OnPropertyChanged(nameof(ValueDEC));
            OnPropertyChanged(nameof(ValueHEX));
            OnPropertyChanged(nameof(ValueOCT));
            OnPropertyChanged(nameof(ValueBIN));
        }
        public void AddHexChar(char c)
        {
            int digitVal = 10 + (char.ToUpper(c) - 'A');
            if (!IsDigitAllowed(digitVal))
                return;
            if (Operation.Contains("="))
                C();
            if (Operator == "")
            {
                if (Number2 == "0" || Number2 == "Cannot divide by zero!")
                    Number2 = digitVal.ToString();
                else
                {
                    BigInteger currentValue;
                    if (!BigInteger.TryParse(Number2, out currentValue))
                        currentValue = 0;
                    BigInteger newValue = currentValue * 16 + digitVal;
                    Number2 = newValue.ToString();
                }
            }
            else
            {
                if (Number == "0")
                {
                    Number = Number2;
                    Number2 = digitVal.ToString();
                }
                else
                {
                    BigInteger currentValue;
                    if (!BigInteger.TryParse(Number2, out currentValue))
                        currentValue = 0;
                    BigInteger newValue = currentValue * 16 + digitVal;
                    Number2 = newValue.ToString();
                }
            }
            OnPropertyChanged(nameof(DisplayNumber));
            OnPropertyChanged(nameof(ValueDEC));
            OnPropertyChanged(nameof(ValueHEX));
            OnPropertyChanged(nameof(ValueOCT));
            OnPropertyChanged(nameof(ValueBIN));
        }
        private bool IsDigitAllowed(int digit)
        {
            switch (SelectedBase)
            {
                case "BIN":
                    return (digit == 0 || digit == 1);
                case "OCT":
                    return (digit >= 0 && digit <= 7);
                case "DEC":
                    return (digit >= 0 && digit <= 9);
                case "HEX":
                    return (digit >= 0 && digit <= 15);
                default:
                    return true;
            }
        }
        public new void AddOperator(string input)
        {
            if (Operation.Contains("="))
            {
                Number = Number2;
                Operation = Number2;
                Operator = "";
            }
            else if (!string.IsNullOrEmpty(Operator))
            {
                ComputeOperation();
            }
            Number = Number2;
            Operator = input;
            Operation = Number + input;
            ConcatenateToNumber2 = false;
            OnPropertyChanged(nameof(DisplayNumber));
        }
        public new void Equals()
        {
            base.Equals();
            OnPropertyChanged(nameof(DisplayNumber));
        }
        public new void C()
        {
            base.C();
            OnPropertyChanged(nameof(DisplayNumber));
        }
    }
}
