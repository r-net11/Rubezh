using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevicesModule.ViewModels;

namespace DevicesModule.Views
{
    public partial class AddressEditor : UserControl
    {
        const char DOT = '.';
        static readonly string MessageFormat = "Недопустимое значение {0}. Укажите значение в диапазоне от {1} до {2}.";

        public AddressEditor()
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
                    return int.Parse(addressEditor.Text.Remove(LeftPartLen));

                return int.Parse(addressEditor.Text);
            }
            set
            {
                if (_isHaveDelimiter)
                    addressEditor.Text = value.ToString(LeftPartFormat) + addressEditor.Text.Substring(LeftPartLen);
                else
                    addressEditor.Text = value.ToString(LeftPartFormat);
            }
        }

        int RightPart
        {
            get
            {
                if (_isHaveDelimiter)
                    return int.Parse(addressEditor.Text.Substring(LeftPartLen + 1));
                return -1;
            }
            set
            {
                if (value > 0 && _isHaveDelimiter)
                    addressEditor.Text = addressEditor.Text.Remove(LeftPartLen + 1) + value.ToString(RightPartFormat);
            }
        }

        bool _isHaveDelimiter;

        void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var deviceViewModel = DataContext as DeviceViewModel;
            SetBounds();

            string text = deviceViewModel.Address;
            if (text.Contains(" "))
                text = text.Substring(0, text.IndexOf(' '));

            if (_isHaveDelimiter = text.Contains(DOT.ToString()))
            {
                addressEditor.MaxLength = LeftPartLen + 1 + RightPartLen;
                addressEditor.Text = text;
                LeftPart = LeftPart;
                RightPart = RightPart;
            }
            else
            {
                addressEditor.MaxLength = LeftPartLen;
                addressEditor.Text = text;
                LeftPart = LeftPart;
            }

            addressEditor.Focus();
            addressEditor.CaretIndex = 0;
        }

        void SetBounds()
        {
            var deviceViewModel = DataContext as DeviceViewModel;
            if (deviceViewModel.Driver.IsRangeEnabled)
            {
                LeftPartMin = deviceViewModel.Driver.MinAddress;
                LeftPartMax = deviceViewModel.Driver.MaxAddress;
            }
            else
            {
                LeftPartMax = deviceViewModel.Device.Parent.Driver.ShleifCount;
                if (LeftPartMax == 0)
                    LeftPartMax = 1;
                if (deviceViewModel.Parent.Driver.IsChildAddressReservedRange && deviceViewModel.Parent.Driver.ChildAddressReserveRangeCount > 0)
                {
                    LeftPartMin = LeftPartMax = deviceViewModel.Parent.Device.IntAddress >> 8;
                    RightPartMin = deviceViewModel.Parent.Device.IntAddress & 0xff;
                    RightPartMax = RightPartMin + deviceViewModel.Parent.Driver.ChildAddressReserveRangeCount;
                }
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
            if (addressEditor.CaretIndex >= addressEditor.Text.Length)
                return;

            addressEditor.CaretIndex += 1;
            if (addressEditor.Text[addressEditor.CaretIndex] == DOT)
                addressEditor.CaretIndex += 1;
        }

        void LeftKeyHandle()
        {
            if (addressEditor.CaretIndex <= 0)
                return;

            if (addressEditor.Text[addressEditor.CaretIndex - 1] == DOT)
                addressEditor.CaretIndex -= 2;
            else
                addressEditor.CaretIndex -= 1;
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckLeftPart();
            CheckRightPart();
        }

        void CheckLeftPart()
        {
            int caretIndex = addressEditor.CaretIndex;
            int leftPart = LeftPart;
            if (leftPart < LeftPartMin)
            {
                DialogBox.DialogBox.Show(
                    string.Format(MessageFormat, leftPart, LeftPartMin, LeftPartMax),
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                LeftPart = LeftPartMin;
                addressEditor.CaretIndex = caretIndex;
            }
            else if (leftPart > LeftPartMax)
            {
                DialogBox.DialogBox.Show(
                    string.Format(MessageFormat, leftPart, LeftPartMin, LeftPartMax),
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                LeftPart = LeftPartMax;
                addressEditor.CaretIndex = caretIndex;
            }
        }

        void CheckRightPart()
        {
            int caretIndex = addressEditor.CaretIndex;

            int rightPart = RightPart;
            if (rightPart < 0)
                return;

            if (rightPart < RightPartMin)
            {
                DialogBox.DialogBox.Show(
                    string.Format(MessageFormat, rightPart, RightPartMin, RightPartMax),
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                RightPart = RightPartMin;
                addressEditor.CaretIndex = caretIndex;
            }
            else if (rightPart > RightPartMax)
            {
                DialogBox.DialogBox.Show(
                    string.Format(MessageFormat, rightPart, RightPartMin, RightPartMax),
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                RightPart = RightPartMax;
                addressEditor.CaretIndex = caretIndex;
            }
        }

        void OnSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (addressEditor.SelectionStart >= addressEditor.Text.Length)
                addressEditor.SelectionStart = addressEditor.Text.Length - 1;

            if (addressEditor.Text[addressEditor.SelectionStart] == DOT)
                addressEditor.SelectionStart += 1;

            if (addressEditor.SelectionLength != 1)
                addressEditor.SelectionLength = 1;
        }

        void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_isHaveDelimiter)
                (DataContext as DeviceViewModel).Address = LeftPart.ToString() + '.' + RightPart.ToString();
            else
                (DataContext as DeviceViewModel).Address = LeftPart.ToString();
        }

        void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        void OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        void OnPreviewDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}