using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using RubezhAPI.GK;

namespace GKModule.Views
{
	public partial class AddressEditor : UserControl
	{
		static readonly string MessageFormat = "Недопустимое значение {0}. Укажите значение в диапазоне от {1} до {2}.";
		bool _isSaving;

		public static readonly DependencyProperty DeviceProperty =
			DependencyProperty.Register("Device", typeof(GKDevice), typeof(AddressEditor),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDevicePropertyChanged)));

		public static readonly DependencyProperty AddressProperty =
			DependencyProperty.Register("Address", typeof(string), typeof(AddressEditor),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnAddressPropertyChanged)));

		public GKDevice Device
		{
			get { return (GKDevice)GetValue(DeviceProperty); }
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
			IntAddressMin = (byte)1;
			IntAddressMax = (byte)255;
		}

		public byte IntAddressMin { get; set; }
		public byte IntAddressMax { get; set; }

		byte IntAddressLength
		{
			get { return (byte)IntAddressMax.ToString().Length; }
		}

		string IntAddressFormat
		{
			get { return string.Format("D{0}", IntAddressLength); }
		}

		int IntAddress
		{
			get
			{
				return int.Parse(addressEditor.Text);
			}
			set
			{
				addressEditor.Text = value.ToString(IntAddressFormat);
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

			addressEditor.Text = Device.IntAddress.ToString(IntAddressFormat);

			addressEditor.MaxLength = IntAddressLength;
			IntAddress = IntAddress;

			addressEditor.Focus();
			addressEditor.CaretIndex = 0;
		}

		void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			SaveAddress();
		}

		void SaveAddress()
		{
			if (Device == null)
				return;
			_isSaving = true;
			Device.IntAddress = (byte)IntAddress;
			Address = IntAddress.ToString();
			_isSaving = false;
		}

		void SetBounds()
		{
			if (Device.Driver.IsRangeEnabled)
			{
				IntAddressMin = Device.Driver.MinAddress;
				IntAddressMax = (byte)Device.Driver.MaxAddress;
			}
		}

		void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			CheckLeftPart();
			SaveAddress();
		}

		void CheckLeftPart()
		{
			int caretIndex = addressEditor.CaretIndex;
			int intAddress = IntAddress;
			if (intAddress < IntAddressMin)
			{
				ShowError(string.Format(MessageFormat, intAddress, IntAddressMin, IntAddressMax));
				IntAddress = IntAddressMin;
			}
			else if (intAddress > IntAddressMax)
			{
				ShowError(string.Format(MessageFormat, intAddress, IntAddressMin, IntAddressMax));
				IntAddress = IntAddressMax;
			}
			addressEditor.CaretIndex = caretIndex;
		}

		void OnRightKey()
		{
			if (addressEditor.CaretIndex < addressEditor.Text.Length)
			{
				addressEditor.CaretIndex += 1;
			}
		}

		void OnLeftKey()
		{
			if (addressEditor.CaretIndex > 0)
			{
				addressEditor.CaretIndex -= 1;
			}
		}

		void OnSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
		{
			if (addressEditor.SelectionStart >= addressEditor.Text.Length)
				addressEditor.SelectionStart = addressEditor.Text.Length - 1;

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

			var dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 3), IsEnabled = true };
			dispatcherTimer.Tick += new EventHandler(delegate(object timerSender, EventArgs timerArgs)
			{
				if (toolTip != null)
				{
					toolTip.IsOpen = false;
				}
				toolTip = null;
				dispatcherTimer = null;
			});
		}
	}
}