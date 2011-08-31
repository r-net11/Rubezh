using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevicesModule.Views
{
    public partial class AddressRedactor : UserControl
    {
        const char DOT = '.';
        static readonly string MessageFormat = "Недопустимое значение {0}. Укажите значение в диапазоне от {1} до {2}.";

        public AddressRedactor()
        {
            InitializeComponent();

            LeftPartMin = 1;
            LeftPartMax = 9;
            RightPartMin = 1;
            RightPartMax = 255;
        }

        public int LeftPartMin { get; set; }
        public int LeftPartMax { get; set; }
        int LeftPartLen
        {
            get { return LeftPartMax.ToString().Length; }
        }
        string LeftPartFormat
        {
            get { return string.Format("D{0}", LeftPartLen); }
        }

        public int RightPartMin { get; set; }
        public int RightPartMax { get; set; }
        int RightPartLen
        {
            get { return RightPartMax.ToString().Length; }
        }
        string RightPartFormat
        {
            get { return string.Format("D{0}", RightPartLen); }
        }

        int LeftPart
        {
            get
            {
                if (_isHaveDelimiter)
                    return int.Parse(addressRedactor.Text.Remove(LeftPartLen));

                return int.Parse(addressRedactor.Text);
            }
            set
            {
                if (_isHaveDelimiter)
                    addressRedactor.Text = value.ToString(LeftPartFormat) + addressRedactor.Text.Substring(LeftPartLen);
                else
                    addressRedactor.Text = value.ToString();
            }
        }

        int RightPart
        {
            get
            {
                if (_isHaveDelimiter)
                    return int.Parse(addressRedactor.Text.Substring(LeftPartLen + 1));

                return -1;
            }
            set
            {
                if (value > 0 && _isHaveDelimiter)
                    addressRedactor.Text = addressRedactor.Text.Remove(LeftPartLen + 1) + value.ToString(RightPartFormat);
            }
        }

        bool _isHaveDelimiter;

        void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            string text = DataContext.ToString();
            if (string.IsNullOrWhiteSpace(text) == false && text != addressRedactor.Text)
            {
                if (_isHaveDelimiter = text.Contains(DOT.ToString()))
                {
                    addressRedactor.MaxLength = LeftPartLen + 1 + RightPartLen;
                    addressRedactor.Text = text;
                    LeftPart = LeftPart;
                    RightPart = RightPart;
                }
                else
                {
                    addressRedactor.MaxLength = LeftPartLen;
                    addressRedactor.Text = text;
                    LeftPart = LeftPart;
                }

                addressRedactor.Focus();
                addressRedactor.CaretIndex = 0;
            }
        }

        bool IsUndesirableKey(Key key)
        {
            if (
                key != System.Windows.Input.Key.D0 &&
                key != System.Windows.Input.Key.D1 &&
                key != System.Windows.Input.Key.D2 &&
                key != System.Windows.Input.Key.D3 &&
                key != System.Windows.Input.Key.D4 &&
                key != System.Windows.Input.Key.D5 &&
                key != System.Windows.Input.Key.D6 &&
                key != System.Windows.Input.Key.D7 &&
                key != System.Windows.Input.Key.D8 &&
                key != System.Windows.Input.Key.D9 &&
                key != System.Windows.Input.Key.NumPad0 &&
                key != System.Windows.Input.Key.NumPad1 &&
                key != System.Windows.Input.Key.NumPad2 &&
                key != System.Windows.Input.Key.NumPad3 &&
                key != System.Windows.Input.Key.NumPad4 &&
                key != System.Windows.Input.Key.NumPad5 &&
                key != System.Windows.Input.Key.NumPad6 &&
                key != System.Windows.Input.Key.NumPad7 &&
                key != System.Windows.Input.Key.NumPad8 &&
                key != System.Windows.Input.Key.NumPad9 &&
                key != System.Windows.Input.Key.Left &&
                key != System.Windows.Input.Key.Right &&
                key != System.Windows.Input.Key.Enter
            ) return true;
            return false;
        }

        void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat == false && IsUndesirableKey(e.Key) == false)
            {
                switch (e.Key)
                {
                    case Key.Right:
                        e.Handled = true;
                        RighKeytHandle();
                        return;

                    case Key.Left:
                        e.Handled = true;
                        LeftKeyHandle();
                        return;

                    default:
                        return;
                }
            }
            e.Handled = true;
        }

        void RighKeytHandle()
        {
            if (addressRedactor.CaretIndex >= addressRedactor.Text.Length)
                return;

            addressRedactor.CaretIndex += 1;
            if (addressRedactor.Text[addressRedactor.CaretIndex] == DOT)
                addressRedactor.CaretIndex += 1;
        }

        void LeftKeyHandle()
        {
            if (addressRedactor.CaretIndex <= 0)
                return;

            if (addressRedactor.Text[addressRedactor.CaretIndex - 1] == DOT)
                addressRedactor.CaretIndex -= 2;
            else
                addressRedactor.CaretIndex -= 1;
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckLeftPart();
            CheckRightPart();
        }

        void CheckLeftPart()
        {
            int caretIndex = addressRedactor.CaretIndex;
            int leftPart = LeftPart;
            if (leftPart < LeftPartMin)
            {
                MessageBox.Show(
                    string.Format(MessageFormat, leftPart, LeftPartMin, LeftPartMax),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                LeftPart = LeftPartMin;
                addressRedactor.CaretIndex = caretIndex;
            }
            else if (leftPart > LeftPartMax)
            {
                MessageBox.Show(
                    string.Format(MessageFormat, leftPart, LeftPartMin, LeftPartMax),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                LeftPart = LeftPartMax;
                addressRedactor.CaretIndex = caretIndex;
            }
        }

        void CheckRightPart()
        {
            int caretIndex = addressRedactor.CaretIndex;

            int rightPart = RightPart;
            if (rightPart < 0)
                return;

            if (rightPart < RightPartMin)
            {
                MessageBox.Show(
                    string.Format(MessageFormat, rightPart, RightPartMin, RightPartMax),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                RightPart = RightPartMin;
                addressRedactor.CaretIndex = caretIndex;
            }
            else if (rightPart > RightPartMax)
            {
                MessageBox.Show(
                    string.Format(MessageFormat, rightPart, RightPartMin, RightPartMax),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                RightPart = RightPartMax;
                addressRedactor.CaretIndex = caretIndex;
            }
        }

        void OnSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (addressRedactor.SelectionStart >= addressRedactor.Text.Length)
                addressRedactor.SelectionStart = addressRedactor.Text.Length - 1;

            if (addressRedactor.Text[addressRedactor.SelectionStart] == DOT)
                addressRedactor.SelectionStart += 1;

            if (addressRedactor.SelectionLength != 1)
                addressRedactor.SelectionLength = 1;
        }

        void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            DataContext = addressRedactor.Text;
        }
    }
}