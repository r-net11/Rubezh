using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using XFiresecAPI;

namespace GKModule.Views
{
	public partial class AddressEditor : UserControl
	{
		static readonly string MessageFormat = "Недопустимое значение {0}. Укажите значение в диапазоне от {1} до {2}.";
		bool _isSaving;

		public static readonly DependencyProperty DeviceProperty =
			DependencyProperty.Register("Device", typeof(XDevice), typeof(AddressEditor),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDevicePropertyChanged)));


		public static readonly DependencyProperty AddressProperty =
			DependencyProperty.Register("Address", typeof(string), typeof(AddressEditor),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnAddressPropertyChanged)));

		public XDevice Device
		{
			get { return (XDevice)GetValue(DeviceProperty); }
			set { SetValue(DeviceProperty, value); }
		}

		public string Address
		{
			get { return (string)GetValue(AddressProperty); }
			set { SetValue(AddressProperty, value); }
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

		public AddressEditor()
		{
			InitializeComponent();

			LeftPartMin = (byte)1;
			LeftPartMax = (byte)8;
			RightPartMin = (byte)1;
			RightPartMax = (byte)255;
		}

		public byte LeftPartMin { get; set; }
		public byte LeftPartMax { get; set; }
		public byte RightPartMin { get; set; }
		public byte RightPartMax { get; set; }

		byte LeftPartLen
		{
			get { return (byte)LeftPartMax.ToString().Length; }
		}
		byte RightPartLen
		{
			get { return (byte)RightPartMax.ToString().Length; }
		}

		string LeftPartFormat
		{
			get { return string.Format("D{0}", LeftPartLen); }
		}
		string RightPartFormat
		{
			get { return string.Format("D{0}", RightPartLen); }
		}

		int LeftPart
		{
			get
			{
				if (Device.Driver.IsDeviceOnShleif)
					return int.Parse(addressEditor.Text.Remove(addressEditor.Text.IndexOf('.')));
				return int.Parse(addressEditor.Text);
			}
			set
			{
				if (Device.Driver.IsDeviceOnShleif)
					addressEditor.Text = value.ToString(LeftPartFormat) + addressEditor.Text.Substring(addressEditor.Text.IndexOf('.'));
				else
					addressEditor.Text = value.ToString(LeftPartFormat);
			}
		}

		int RightPart
		{
			get
			{
				if (Device.Driver.IsDeviceOnShleif)
					return int.Parse(addressEditor.Text.Substring(addressEditor.Text.IndexOf('.') + 1));
				return 0;
			}
			set
			{
				if (value > 0 && Device.Driver.IsDeviceOnShleif)
					addressEditor.Text = addressEditor.Text.Remove(addressEditor.Text.IndexOf('.') + 1) + value.ToString(RightPartFormat);
			}
		}

		void InitializeDevice()
		{
			if (Device.CanEditAddress == false)
				return;

			SetBounds();
		}

		void InitializeAddress()
		{
			if (Device.CanEditAddress == false)
				return;

			if (_isSaving)
				return;

			string text = "";
			if (Device.Driver.IsDeviceOnShleif)
				text = Device.ShleifNo.ToString() + "." + Device.IntAddress.ToString(RightPartFormat);
			else
				text = Device.IntAddress.ToString(LeftPartFormat);

			addressEditor.Text = text;

			if (Device.Driver.IsDeviceOnShleif)
			{
				addressEditor.MaxLength = LeftPartLen + 1 + RightPartLen;
				LeftPart = LeftPart;
				RightPart = RightPart;
			}
			else
			{
				addressEditor.MaxLength = LeftPartLen;
				LeftPart = LeftPart;
			}

			addressEditor.Focus();
			addressEditor.CaretIndex = 0;
		}

		void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			SaveAddress();
		}

		void SaveAddress()
		{
			_isSaving = true;
			if (Device.Driver.IsDeviceOnShleif)
			{
				Device.ShleifNo = (byte)LeftPart;
				Device.IntAddress = (byte)RightPart;
				Address = LeftPart.ToString() + '.' + RightPart.ToString();
			}
			else
			{
				Device.IntAddress = (byte)LeftPart;
				Address = LeftPart.ToString();
			}
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
				LeftPartMin = 1;
				LeftPartMax = 8;
				RightPartMin = 1;
				RightPartMax = 255;
				if (Device.Driver.MaxAddressOnShleif != 0)
					RightPartMax = Device.Driver.MaxAddressOnShleif;
				if (Device.Parent.Driver.IsGroupDevice)
				{
					RightPartMin = Device.Parent.IntAddress;
					RightPartMax = (byte)(RightPartMin + Device.Parent.Driver.GroupDeviceChildrenCount);
				}
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

			if (RightPart < RightPartMin)
			{
				ShowError(string.Format(MessageFormat, RightPart, RightPartMin, RightPartMax));
				RightPart = RightPartMin;
			}
			else if (RightPart > RightPartMax)
			{
				ShowError(string.Format(MessageFormat, RightPart, RightPartMin, RightPartMax));
				RightPart = RightPartMax;
			}
			addressEditor.CaretIndex = caretIndex;
		}

		void OnRightKey()
		{
			if (addressEditor.CaretIndex >= addressEditor.Text.Length)
				return;

			addressEditor.CaretIndex += 1;
			if (addressEditor.Text[addressEditor.CaretIndex] == '.')
				addressEditor.CaretIndex += 1;
		}

		void OnLeftKey()
		{
			if (addressEditor.CaretIndex <= 0)
				return;

			if (addressEditor.Text[addressEditor.CaretIndex - 1] == '.')
				addressEditor.CaretIndex -= 2;
			else
				addressEditor.CaretIndex -= 1;
		}

		void OnSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
		{
			if (addressEditor.SelectionStart >= addressEditor.Text.Length)
				addressEditor.SelectionStart = addressEditor.Text.Length - 1;

			if (addressEditor.Text[addressEditor.SelectionStart] == '.')
				addressEditor.SelectionStart += 1;

			if (addressEditor.SelectionLength != 1)
				addressEditor.SelectionLength = 1;
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
			)
				return true;
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
						OnRightKey();
						return;

					case Key.Left:
						e.Handled = true;
						OnLeftKey();
						return;

					default:
						return;
				}
			}
			if (e.Key != Key.Escape)
				e.Handled = true;
		}

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
	}
}