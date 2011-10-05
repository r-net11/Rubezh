using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SecurityModule.Views
{
    public partial class RemoteMachineView : UserControl
    {
        const char DOT = '.';
        static readonly string MessageFormat = "Недопустимое значение {0}. Укажите значение в диапазоне от {1} до {2}.";
        List<IpAddressByte> IpAddressBytes = new List<IpAddressByte>();

        public RemoteMachineView()
        {
            InitializeComponent();

            IpAddressBytes.Add(new IpAddressByte(1, 223));
            IpAddressBytes.Add(new IpAddressByte(0, 225));
            IpAddressBytes.Add(new IpAddressByte(0, 225));
            IpAddressBytes.Add(new IpAddressByte(0, 225));

            int maxLen = -1;
            IpAddressBytes.ForEach(x => maxLen += x.Len + 1);
            addressRedactor.MaxLength = maxLen;
        }

        void addressRedactor_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue)
            {
                addressRedactor.Text = "001.000.000.000";
            }
            else
            {
                addressRedactor.Text = "";
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
            if (addressRedactor.Text.Length != addressRedactor.MaxLength)
                return;

            for (int i = 0; i < IpAddressBytes.Count; ++i)
                CheckIpAddressByte(i);
        }

        void CheckIpAddressByte(int index)
        {
            int caretIndex = addressRedactor.CaretIndex;
            int ipAddressByte = GetIpAddressByte(index);
            if (ipAddressByte < IpAddressBytes[index].Min)
            {
                DialogBox.DialogBox.Show(
                    string.Format(MessageFormat, ipAddressByte, IpAddressBytes[index].Min, IpAddressBytes[index].Max),
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                SetIpAddressByte(index, IpAddressBytes[index].Min);
                addressRedactor.CaretIndex = caretIndex;
            }
            else if (ipAddressByte > IpAddressBytes[index].Max)
            {
                DialogBox.DialogBox.Show(
                    string.Format(MessageFormat, ipAddressByte, IpAddressBytes[index].Min, IpAddressBytes[index].Max),
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                SetIpAddressByte(index, IpAddressBytes[index].Max);
                addressRedactor.CaretIndex = caretIndex;
            }
        }

        int GetIpAddressByte(int index)
        {
            switch (index)
            {
                case 0:
                    return int.Parse(addressRedactor.Text.Remove(3));

                case 1:
                    return int.Parse(addressRedactor.Text.Substring(4, 3));

                case 2:
                    return int.Parse(addressRedactor.Text.Substring(8, 3));

                case 3:
                    return int.Parse(addressRedactor.Text.Substring(12, 3));

                default:
                    return 0;
            }
        }

        void SetIpAddressByte(int index, int value)
        {
            switch (index)
            {
                case 0:
                    addressRedactor.Text = value.ToString(IpAddressBytes[index].Format) + addressRedactor.Text.Substring(3);
                    break;

                case 1:
                    addressRedactor.Text = addressRedactor.Text.Remove(4) + value.ToString(IpAddressBytes[index].Format) + addressRedactor.Text.Substring(7);
                    break;

                case 2:
                    addressRedactor.Text = addressRedactor.Text.Remove(8) + value.ToString(IpAddressBytes[index].Format) + addressRedactor.Text.Substring(11);
                    break;

                case 3:
                    addressRedactor.Text = addressRedactor.Text.Remove(12) + value.ToString(IpAddressBytes[index].Format);
                    break;

                default:
                    break;
            }
        }

        void OnSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (addressRedactor.Text.Length != addressRedactor.MaxLength)
                return;

            if (addressRedactor.SelectionStart >= addressRedactor.Text.Length)
                addressRedactor.SelectionStart = addressRedactor.Text.Length - 1;

            if (addressRedactor.Text[addressRedactor.SelectionStart] == DOT)
                addressRedactor.SelectionStart += 1;

            if (addressRedactor.SelectionLength != 1)
                addressRedactor.SelectionLength = 1;
        }

        void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            addressRedactor.Text = string.Format("{0}.{1}.{2}.{3}", GetIpAddressByte(0), GetIpAddressByte(1), GetIpAddressByte(2), GetIpAddressByte(3));
        }
    }

    internal class IpAddressByte
    {
        public IpAddressByte(int minValue, int maxValue)
        {
            Min = minValue;
            Max = maxValue;
            Len = maxValue.ToString().Length;
            Format = string.Format("D{0}", Len);
        }

        public int Min { get; private set; }
        public int Max { get; private set; }
        public int Len { get; private set; }
        public string Format { get; private set; }
    }
}