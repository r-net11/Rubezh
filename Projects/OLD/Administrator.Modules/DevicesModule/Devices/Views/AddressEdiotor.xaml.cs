﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using FiresecAPI.Models;

namespace DevicesModule.Views
{
	public partial class AddressEditor : UserControl
	{
		public static readonly DependencyProperty DeviceProperty =
			DependencyProperty.Register("Device", typeof(Device), typeof(AddressEditor),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDevicePropertyChanged)));

		public static readonly DependencyProperty AddressProperty =
			DependencyProperty.Register("Ip", typeof(string), typeof(AddressEditor),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnAddressPropertyChanged)));

		public static readonly DependencyProperty CanEditAddressProperty =
		   DependencyProperty.Register("CanEditAddress", typeof(string), typeof(AddressEditor),
		   new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public bool CanEditAddress
		{
			get { return (bool)GetValue(CanEditAddressProperty); }
			set { SetValue(CanEditAddressProperty, value); }
		}

		public Device Device
		{
			get { return (Device)GetValue(DeviceProperty); }
			set { SetValue(DeviceProperty, value); }
		}

		public string Address
		{
			get { return (string)GetValue(AddressProperty); }
			set { SetValue(AddressProperty, value); }
		}

		const char DOT = '.';
		static readonly string MessageFormat = "Недопустимое значение {0}. Укажите значение в диапазоне от {1} до {2}.";
		bool _hasDelimiter;
		bool _isSaving;

		public AddressEditor()
		{
			InitializeComponent();

			LeftPartMin = 1;
			LeftPartMax = 10;
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
				if (_hasDelimiter)
					return int.Parse(addressEditor.Text.Remove(addressEditor.Text.IndexOf('.')));
				return int.Parse(addressEditor.Text);
			}
			set
			{
				if (_hasDelimiter)
					addressEditor.Text = value.ToString(LeftPartFormat) + addressEditor.Text.Substring(addressEditor.Text.IndexOf('.'));
				else
					addressEditor.Text = value.ToString(LeftPartFormat);
			}
		}

		int RightPart
		{
			get
			{
				if (_hasDelimiter)
					return int.Parse(addressEditor.Text.Substring(addressEditor.Text.IndexOf('.') + 1));
				return -1;
			}
			set
			{
				if (value > 0 && _hasDelimiter)
					addressEditor.Text = addressEditor.Text.Remove(addressEditor.Text.IndexOf('.') + 1) + value.ToString();
					//addressEditor.Text = addressEditor.Text.Remove(addressEditor.Text.IndexOf('.') + 1) + value.ToString(RightPartFormat);
			}
		}

		private static void OnDevicePropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			AddressEditor addressEditor = dp as AddressEditor;
			if (addressEditor != null)
			{
				addressEditor.InitializeDevice();
			}
		}

		private static void OnAddressPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			AddressEditor addressEditor = dp as AddressEditor;
			if (addressEditor != null)
			{
				addressEditor.InitializeAddress();
			}
		}

		void InitializeDevice()
		{
			if (Device == null || Device.CanEditAddress == false)
				return;
			SetBounds();
		}

		void InitializeAddress()
		{
			if (Device == null || Device.CanEditAddress == false)
				return;
			if (_isSaving)
				return;

			string text = Address;
			if (text.Contains(" "))
				text = text.Substring(0, text.IndexOf(' '));

			if (_hasDelimiter = text.Contains(DOT.ToString()))
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
			addressEditor.SelectionStart = 0;
			addressEditor.SelectionLength = 1;
		}

		void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			SaveAddress();
		}

		void SaveAddress()
		{
			_isSaving = true;
			if (_hasDelimiter)
				Address = LeftPart.ToString() + '.' + RightPart.ToString();
			else
				Address = LeftPart.ToString();
			_isSaving = false;
		}

		void SetBounds()
		{
			if (Device.Driver.IsRangeEnabled)
			{
				LeftPartMin = Device.Driver.MinAddress;
				LeftPartMax = Device.Driver.MaxAddress;
			}
			else
			{
				LeftPartMax = Device.ParentPanel.Driver.ShleifCount;
				if (LeftPartMax == 0)
					LeftPartMax = 1;
				if (Device.Parent.Driver.IsChildAddressReservedRange)
				{
					LeftPartMin = LeftPartMax = Device.Parent.IntAddress >> 8;
					LeftPartMax = LeftPartMax = Device.Parent.IntAddress >> 8;
					RightPartMin = Device.Parent.IntAddress & 0xff;
					RightPartMax = RightPartMin + Device.Parent.GetReservedCount();
				}
			}
		}

		bool IsUndesirableKey(Key key)
		{
			switch(key)
			{
				case System.Windows.Input.Key.D0:
				case System.Windows.Input.Key.D1:
				case System.Windows.Input.Key.D2:
				case System.Windows.Input.Key.D3:
				case System.Windows.Input.Key.D4:
				case System.Windows.Input.Key.D5:
				case System.Windows.Input.Key.D6:
				case System.Windows.Input.Key.D7:
				case System.Windows.Input.Key.D8:
				case System.Windows.Input.Key.D9:
				case System.Windows.Input.Key.NumPad0:
				case System.Windows.Input.Key.NumPad1:
				case System.Windows.Input.Key.NumPad2:
				case System.Windows.Input.Key.NumPad3:
				case System.Windows.Input.Key.NumPad4:
				case System.Windows.Input.Key.NumPad5:
				case System.Windows.Input.Key.NumPad6:
				case System.Windows.Input.Key.NumPad7:
				case System.Windows.Input.Key.NumPad8:
				case System.Windows.Input.Key.NumPad9:
				case System.Windows.Input.Key.Left:
				case System.Windows.Input.Key.Right:
				case System.Windows.Input.Key.Enter:
				case System.Windows.Input.Key.Delete:
				case System.Windows.Input.Key.Back:
					return true;
			}
			return false;
		}

		void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (IsUndesirableKey(e.Key))
			{
				switch (e.Key)
				{
					case Key.Right:
						e.Handled = true;
						OnRightKey();
						return;

					case Key.Left:
						e.Handled = true;
						OnLeftKey();
						return;

					case Key.Delete:
						e.Handled = true;
						OnDeleteKey();
						return;

					case Key.Back:
						e.Handled = true;
						OnBackspaceKey();
						return;

					default:
						return;
				}
			}
			if (e.Key != Key.Escape)
				e.Handled = true;
		}

		void OnRightKey()
		{
			if (addressEditor.CaretIndex >= addressEditor.Text.Length)
				return;

			addressEditor.CaretIndex += 1;

			if (addressEditor.Text.Length > addressEditor.CaretIndex)
			if (addressEditor.Text[addressEditor.CaretIndex] == DOT)
				addressEditor.CaretIndex += 1;
		}

		void OnLeftKey()
		{
			if (addressEditor.CaretIndex <= 0)
				return;

			if (addressEditor.Text.Length > addressEditor.CaretIndex - 1)
			if (addressEditor.Text[addressEditor.CaretIndex - 1] == DOT)
				addressEditor.CaretIndex -= 2;
			else
				addressEditor.CaretIndex -= 1;
		}

		void OnDeleteKey()
		{
			var oldCaretIndex = addressEditor.CaretIndex;
			if (addressEditor.CaretIndex < 2)
				return;

			if (addressEditor.Text.IndexOf(DOT) != -1 && addressEditor.Text.Length - addressEditor.Text.IndexOf(DOT) > 2)
			{
				if (addressEditor.Text.Length > addressEditor.CaretIndex)
					addressEditor.Text = addressEditor.Text.Remove(addressEditor.CaretIndex, 1);
				addressEditor.CaretIndex = oldCaretIndex;
			}
		}

		void OnBackspaceKey()
		{
			var oldCaretIndex = addressEditor.CaretIndex;
			if (addressEditor.CaretIndex < 3)
				return;

			if (addressEditor.Text.IndexOf(DOT) != -1 && addressEditor.Text.Length - addressEditor.Text.IndexOf(DOT) > 2)
			{
				if (addressEditor.Text.Length > addressEditor.CaretIndex - 1)
					addressEditor.Text = addressEditor.Text.Remove(addressEditor.CaretIndex - 1, 1);
				addressEditor.CaretIndex = oldCaretIndex;
			}
		}

		void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			CheckLeftPart();
			CheckRightPart();
			SaveAddress();
		}

		void CheckLeftPart()
		{
			int caretIndex = addressEditor.CaretIndex;
			int leftPart = LeftPart;
			if (leftPart < LeftPartMin)
			{
				ShowError(string.Format(MessageFormat, leftPart, LeftPartMin, LeftPartMax));
				LeftPart = LeftPartMin;
			}
			else if (leftPart > LeftPartMax)
			{
				ShowError(string.Format(MessageFormat, leftPart, LeftPartMin, LeftPartMax));
				LeftPart = LeftPartMax;
			}
			addressEditor.CaretIndex = caretIndex;
		}

		void CheckRightPart()
		{
			int caretIndex = addressEditor.CaretIndex;

			int rightPart = RightPart;
			if (rightPart < 0)
				return;

			if (rightPart < RightPartMin)
			{
				ShowError(string.Format(MessageFormat, rightPart, RightPartMin, RightPartMax));
				RightPart = RightPartMin;
			}
			else if (rightPart > RightPartMax)
			{
				ShowError(string.Format(MessageFormat, rightPart, RightPartMin, RightPartMax));
				RightPart = RightPartMax;
			}
			addressEditor.CaretIndex = caretIndex;
		}

		void OnSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!buisy)
			{
				buisy = true;

				//if (addressEditor.SelectionStart >= addressEditor.Text.Length)
				//	addressEditor.SelectionStart = addressEditor.Text.Length - 1;

				if (addressEditor.Text.Length > addressEditor.SelectionStart)
				{
					if (addressEditor.Text[addressEditor.SelectionStart] == DOT)
						addressEditor.SelectionStart += 1;
				}

				if (addressEditor.SelectionLength != 1)
					addressEditor.SelectionLength = 1;
				buisy = false;
			}
		}

		bool buisy = false;

		void OnPreviewHandled(object sender, DragEventArgs e)
		{
			e.Handled = true;
		}

		void ShowError(string message)
		{
			var toolTip = new ToolTip()
			{
				Content = message,
				PlacementTarget = addressEditor,
				Placement = PlacementMode.Top,
				IsOpen = true
			};
			addressEditor.ToolTip = toolTip;

			DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 3), IsEnabled = true };
			timer.Tick += new EventHandler(delegate(object timerSender, EventArgs timerArgs)
			{
				if (toolTip != null)
				{
					toolTip.IsOpen = false;
				}
				toolTip = null;
				timer = null;
			});
		}

		private void addressEditor_GotFocus(object sender, RoutedEventArgs e)
		{
			//addressEditor.CaretIndex = 0;
			//addressEditor.SelectionLength = 1;
		}
	}
}