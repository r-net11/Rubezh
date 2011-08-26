using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevicesModule.Views
{
    public partial class AdressPresenter : UserControl
    {
        public AdressPresenter()
        {
            InitializeComponent();

            LeftPartMin = 1;
            LeftPartMax = 9;
            RightPartMin = 1;
            RightPartMax = 255;
        }

        string messageFormat = "Недопустимое значение {0}. Укажите значение в диапазоне от {1} до {2}.";

        public int LeftPartMin { get; set; }
        public int RightPartMin { get; set; }

        string _leftPartFormat = "";
        int _leftPartLen;
        int _leftPartMax;
        public int LeftPartMax
        {
            get { return _leftPartMax; }
            set
            {
                _leftPartMax = value;
                _leftPartLen = _leftPartMax.ToString().Length;
                _leftPartFormat = string.Format("D{0}", _leftPartLen);
            }
        }

        string _rightPartFormat = "";
        int _rightPartLen;
        int _rightPartMax;
        public int RightPartMax
        {
            get { return _rightPartMax; }
            set
            {
                _rightPartMax = value;
                _rightPartLen = _rightPartMax.ToString().Length;
                _rightPartFormat = string.Format("D{0}", _rightPartLen);
            }
        }

        bool _isHaveDelimiter;

        void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            string text = DataContext.ToString();
            if (string.IsNullOrWhiteSpace(text) == false && text != address.Text)
            {
                if (text.Contains("."))
                {
                    _isHaveDelimiter = true;
                    address.MaxLength = _leftPartLen + 1 + _rightPartLen;
                    address.Text = text;
                    SetLeftPart(GetLeftPart());
                    SetRightPart((int) GetRightPart());
                }
                else
                {
                    _isHaveDelimiter = false;
                    address.MaxLength = _leftPartLen;
                    address.Text = text;
                    SetLeftPart(GetLeftPart());
                }

                address.Focus();
                address.CaretIndex = 0;
            }
        }

        bool IsUndesirableKey(Key key)
        {
            if (key != System.Windows.Input.Key.D0 &&
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
                key != System.Windows.Input.Key.Enter)
            {
                return true;
            }

            return false;
        }

        void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat || IsUndesirableKey(e.Key))
            {
                e.Handled = true;
                return;
            }

            switch (e.Key)
            {
                case Key.Right:
                    e.Handled = true;
                    RighKeytHandle();
                    break;

                case Key.Left:
                    e.Handled = true;
                    LeftKeyHandle();
                    break;
            }
        }

        void RighKeytHandle()
        {
            if (address.CaretIndex < address.Text.Length)
            {
                address.CaretIndex += 1;
                if (address.Text[address.CaretIndex] == '.')
                    address.CaretIndex += 1;
            }
        }

        void LeftKeyHandle()
        {
            if (address.CaretIndex > 0)
            {
                if (address.Text[address.CaretIndex - 1] == '.')
                    address.CaretIndex -= 2;
                else
                    address.CaretIndex -= 1;
            }
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckLeftPart();
            CheckRightPart();
        }

        void CheckLeftPart()
        {
            int firstPart = GetLeftPart();
            if (firstPart < LeftPartMin)
            {
                MessageBox.Show(
                    string.Format(messageFormat, firstPart, LeftPartMin, LeftPartMax),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                int caretIndex = address.CaretIndex;
                SetLeftPart(LeftPartMin);
                address.CaretIndex = caretIndex;
            }
            else if (firstPart > LeftPartMax)
            {
                MessageBox.Show(
                    string.Format(messageFormat, firstPart, LeftPartMin, LeftPartMax),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                int caretIndex = address.CaretIndex;
                SetLeftPart(LeftPartMax);
                address.CaretIndex = caretIndex;
            }
        }

        void CheckRightPart()
        {
            int? secontPart = GetRightPart();
            if (secontPart == null)
                return;

            if (secontPart < RightPartMin)
            {
                MessageBox.Show(
                    string.Format(messageFormat, secontPart, RightPartMin, RightPartMax),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                int caretIndex = address.CaretIndex;
                SetRightPart(RightPartMin);
                address.CaretIndex = caretIndex;
            }
            else if (secontPart > RightPartMax)
            {
                MessageBox.Show(
                    string.Format(messageFormat, secontPart, RightPartMin, RightPartMax),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                int caretIndex = address.CaretIndex;
                SetRightPart(RightPartMax);
                address.CaretIndex = caretIndex;
            }
        }

        int GetLeftPart()
        {
            if (_isHaveDelimiter)
                return int.Parse(address.Text.Remove(address.Text.IndexOf('.')));

            return int.Parse(address.Text);
        }

        int? GetRightPart()
        {
            if (_isHaveDelimiter)
                return int.Parse(address.Text.Substring(address.Text.IndexOf('.') + 1));

            return null;
        }

        void SetLeftPart(int value)
        {
            if (_isHaveDelimiter)
            {
                string leftPart = value.ToString(_leftPartFormat);
                address.Text = leftPart + address.Text.Substring(address.Text.IndexOf('.'));
            }
            else
            {
                address.Text = value.ToString();
            }
        }

        void SetRightPart(int value)
        {
            if (_isHaveDelimiter)
            {
                string rightPart = value.ToString(_rightPartFormat);
                address.Text = address.Text.Remove(address.Text.IndexOf('.') + 1) + rightPart;
            }
        }

        void OnSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (address.SelectionStart >= address.Text.Length)
                address.SelectionStart = address.Text.Length - 1;

            if (address.Text[address.SelectionStart] == '.')
                address.SelectionStart += 1;

            if (address.SelectionLength != 1)
                address.SelectionLength = 1;
        }

        void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            DataContext = address.Text;
        }
    }
}