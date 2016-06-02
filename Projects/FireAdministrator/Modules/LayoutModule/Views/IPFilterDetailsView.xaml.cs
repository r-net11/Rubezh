using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common;
using Infrastructure.Common.Windows;
using Localization.Layout.Errors;

namespace LayoutModule.Views
{
	public partial class IPFilterDetailsView : UserControl
	{
		const char DOT = '.';
        static readonly string MessageFormat = CommonErrors.IpFilterDetailsView_Error;
		List<IpAddressByte> IpAddressBytes = new List<IpAddressByte>();

		public IPFilterDetailsView()
		{
			InitializeComponent();

			IpAddressBytes.Add(new IpAddressByte(1, 223));
			IpAddressBytes.Add(new IpAddressByte(0, 225));
			IpAddressBytes.Add(new IpAddressByte(0, 225));
			IpAddressBytes.Add(new IpAddressByte(0, 225));

			int maxLen = -1;
			IpAddressBytes.ForEach(x => maxLen += x.Len + 1);
			addressEditor.MaxLength = maxLen;
		}

		void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
				addressEditor.Text = "001.000.000.000";
			else
				addressEditor.Text = "";
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
			if (addressEditor.Text.Length != addressEditor.MaxLength)
				return;
			try
			{
				for (int i = 0; i < IpAddressBytes.Count; i++)
					CheckIpAddressByte(i);
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "Исключение при вызове RemoteMachineView.OnTextChanged");
			}
		}

		void CheckIpAddressByte(int index)
		{
			int caretIndex = addressEditor.CaretIndex;
			int ipAddressByte = GetIpAddressByte(index);
			if (ipAddressByte < IpAddressBytes[index].Min)
			{
				MessageBoxService.ShowWarning(string.Format(MessageFormat, ipAddressByte, IpAddressBytes[index].Min, IpAddressBytes[index].Max));

				SetIpAddressByte(index, IpAddressBytes[index].Min);
				addressEditor.CaretIndex = caretIndex;
			}
			else if (ipAddressByte > IpAddressBytes[index].Max)
			{
				MessageBoxService.ShowWarning(string.Format(MessageFormat, ipAddressByte, IpAddressBytes[index].Min, IpAddressBytes[index].Max));

				SetIpAddressByte(index, IpAddressBytes[index].Max);
				addressEditor.CaretIndex = caretIndex;
			}
		}

		int GetIpAddressByte(int index)
		{
			switch (index)
			{
				case 0:
					return int.Parse(addressEditor.Text.Remove(3));

				case 1:
					return int.Parse(addressEditor.Text.Substring(4, 3));

				case 2:
					return int.Parse(addressEditor.Text.Substring(8, 3));

				case 3:
					return int.Parse(addressEditor.Text.Substring(12, 3));

				default:
					return 0;
			}
		}

		void SetIpAddressByte(int index, int value)
		{
			switch (index)
			{
				case 0:
					addressEditor.Text = value.ToString(IpAddressBytes[index].Format) + addressEditor.Text.Substring(3);
					break;

				case 1:
					addressEditor.Text = addressEditor.Text.Remove(4) + value.ToString(IpAddressBytes[index].Format) + addressEditor.Text.Substring(7);
					break;

				case 2:
					addressEditor.Text = addressEditor.Text.Remove(8) + value.ToString(IpAddressBytes[index].Format) + addressEditor.Text.Substring(11);
					break;

				case 3:
					addressEditor.Text = addressEditor.Text.Remove(12) + value.ToString(IpAddressBytes[index].Format);
					break;

				default:
					break;
			}
		}

		void OnSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
		{
			if (addressEditor.Text.Length != addressEditor.MaxLength)
				return;

			if (addressEditor.SelectionStart >= addressEditor.Text.Length)
				addressEditor.SelectionStart = addressEditor.Text.Length - 1;

			if (addressEditor.Text[addressEditor.SelectionStart] == DOT)
				addressEditor.SelectionStart += 1;

			if (addressEditor.SelectionLength != 1)
				addressEditor.SelectionLength = 1;
		}

		void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				addressEditor.Text = string.Format("{0}.{1}.{2}.{3}", GetIpAddressByte(0), GetIpAddressByte(1), GetIpAddressByte(2), GetIpAddressByte(3));
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "Исключение при вызове RemoteMachineView.OnLostFocus");
				addressEditor.Text = "";
			}
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