using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Controls
{
	public partial class TimePicker : UserControl
	{
		public static readonly DependencyProperty TimeSpanProperty = DependencyProperty.Register("TimeSpan", typeof(TimeSpan), typeof(TimePicker),
			new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnTimeSpanPropertyChanged)));

		public static readonly DependencyProperty MinTimeSpanProperty = DependencyProperty.Register("MinTimeSpan", typeof(TimeSpan), typeof(TimePicker),
			new FrameworkPropertyMetadata(DateTime.MinValue.TimeOfDay/*, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault*/));

		public static readonly DependencyProperty MaxTimeSpanProperty = DependencyProperty.Register("MaxTimeSpan", typeof(TimeSpan), typeof(TimePicker),
			new FrameworkPropertyMetadata(DateTime.MaxValue.TimeOfDay/*, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault*/));

		private static void OnTimeSpanPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			TimePicker timePicker = dp as TimePicker;
			if (timePicker != null)
			{
				//timePicker.TimeSpan = timePicker.TimeSpan;
				timePicker.TextBox.Text = String.Format("{0}:{1}", timePicker.TimeSpan.Hours.ToString("D2"), timePicker.TimeSpan.Minutes.ToString("D2"));
			}
			dp.CoerceValue(TimeSpanProperty);
		}

		public TimePicker()
		{
			InitializeComponent();
			InitializeTime();
			DataObject.AddCopyingHandler(TextBox, (sender, e) => { if (e.IsDragDrop) e.CancelCommand(); });
		}

		public TimeSpan MinTimeSpan
		{
			get { return (TimeSpan)GetValue(MinTimeSpanProperty); }
			set { SetValue(MinTimeSpanProperty, value); }
		}

		private int HoursMin
		{
			get { return MinTimeSpan.Hours; }
		}

		private int MinutesMin
		{
			get { return MinTimeSpan.Minutes; }
		}

		public TimeSpan MaxTimeSpan
		{
			get { return (TimeSpan)GetValue(MaxTimeSpanProperty); }
			set { SetValue(MaxTimeSpanProperty, value); }
		}

		private int HoursMax
		{
			get { return MaxTimeSpan.Hours; }
		}

		private int MinutesMax
		{
			get { return MaxTimeSpan.Minutes; }
		}

		public TimeSpan TimeSpan
		{
			get { return (TimeSpan)GetValue(TimeSpanProperty); }
			set { SetValue(TimeSpanProperty, value); }
		}

		public void InitializeTime()
		{
			//if (TextBox.Text.Length != 5)
			//	TextBox.Text = "00:00";
			TextBox.Text = String.Format("{0}:{1}", TimeSpan.Hours.ToString("D2"), TimeSpan.Minutes.ToString("D2"));
			Hours = TimeSpan.Hours;
			Minutes = TimeSpan.Minutes;
		}

		private int Hours
		{
			get
			{
				int result = 0;
				int.TryParse(TextBox.Text.Substring(0, 2), out result);
				return result;
			}
			set
			{
				if (value > HoursMax)
					value = HoursMax;
				if (value < HoursMin)
					value = HoursMin;
				var stringValue = value.ToString("D2");
				var text = TextBox.Text.ToCharArray();
				var caretIndex = TextBox.CaretIndex;
				text[0] = stringValue[0];
				text[1] = stringValue[1];
				TextBox.Text = new string(text);
				TextBox.CaretIndex = caretIndex;
				TimeSpan = new TimeSpan(value, TimeSpan.Minutes, 0);
			}
		}

		private int Minutes
		{
			get
			{
				int result = 0;
				if (TextBox.Text.Length >= 5)
				{
					int.TryParse(TextBox.Text.Substring(3, 2), out result);
				}
				return result;
			}
			set
			{
				if (value > MinutesMax)
					value = MinutesMax;
				if (value < MinutesMin)
					value = MinutesMin;
				var stringValue = value.ToString("D2");
				var text = TextBox.Text.ToCharArray();
				var caretIndex = TextBox.CaretIndex;
				text[3] = stringValue[0];
				text[4] = stringValue[1];
				TextBox.Text = new string(text);
				TextBox.CaretIndex = caretIndex;
				TimeSpan = new TimeSpan(TimeSpan.Hours, value, 0);
			}
		}

		private bool IsDigitKey(Key key)
		{
			if (
				key == System.Windows.Input.Key.D0 ||
				key == System.Windows.Input.Key.D1 ||
				key == System.Windows.Input.Key.D2 ||
				key == System.Windows.Input.Key.D3 ||
				key == System.Windows.Input.Key.D4 ||
				key == System.Windows.Input.Key.D5 ||
				key == System.Windows.Input.Key.D6 ||
				key == System.Windows.Input.Key.D7 ||
				key == System.Windows.Input.Key.D8 ||
				key == System.Windows.Input.Key.D9 ||
				key == System.Windows.Input.Key.NumPad0 ||
				key == System.Windows.Input.Key.NumPad1 ||
				key == System.Windows.Input.Key.NumPad2 ||
				key == System.Windows.Input.Key.NumPad3 ||
				key == System.Windows.Input.Key.NumPad4 ||
				key == System.Windows.Input.Key.NumPad5 ||
				key == System.Windows.Input.Key.NumPad6 ||
				key == System.Windows.Input.Key.NumPad7 ||
				key == System.Windows.Input.Key.NumPad8 ||
				key == System.Windows.Input.Key.NumPad9
			)
				return true;
			return false;
		}

		private void SetValueAtIndex(int value, int caretIndex)
		{
			if (caretIndex == 2)
				return;
			var text = TextBox.Text.ToCharArray();
			if (value < 0)
				value = 0;
			if (caretIndex == 0 && value > 2)
				value = 2;
			if (caretIndex == 3 && value > 5)
				value = 5;
			if ((caretIndex == 4 || caretIndex == 1) && value > 9)
				value = 9;
			text[TextBox.CaretIndex] = value.ToString()[0];
			TextBox.Text = new string(text);
			TextBox.CaretIndex = caretIndex;
			TimeSpan = new TimeSpan(Hours, Minutes, 0);
		}

		private int GetValueAtIndex(int caretIndex)
		{
			if (caretIndex == 2)
				return -1;
			return (int)Char.GetNumericValue(TextBox.Text.ToCharArray()[caretIndex]);
		}

		private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
		{
			if (TextBox.Text.Length == 0)
				return;
			if (TextBox.SelectionStart >= TextBox.Text.Length)
				TextBox.SelectionStart = TextBox.Text.Length - 1;
			if (TextBox.CaretIndex == 2)
				TextBox.CaretIndex = 3;
			if (TextBox.SelectionLength != 1)
				TextBox.SelectionLength = 1;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Hours > HoursMax)
				Hours = HoursMax;
			if (Hours < HoursMin)
				Hours = HoursMin;
			if (Minutes > MinutesMax)
				Minutes = MinutesMax;
			if (Minutes < MinutesMin)
				Minutes = MinutesMin;
			TimeSpan = new TimeSpan(Hours, Minutes, 0);
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			TimeSpan = new TimeSpan(Hours, Minutes, 0);
		}

		private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (IsDigitKey(e.Key))
			{
				if (TextBox.CaretIndex == 2)
					e.Handled = true;
				return;
			}
			var caretIndex = TextBox.CaretIndex;
			switch (e.Key)
			{
				case (Key.Left):
					if (TextBox.CaretIndex == 3)
						TextBox.CaretIndex -= 2;
					else if (TextBox.CaretIndex > 0)
						TextBox.CaretIndex--;
					e.Handled = true;
					break;

				case (Key.Right):
					if (TextBox.CaretIndex == 1)
						TextBox.CaretIndex += 2;
					else if (TextBox.CaretIndex < TextBox.Text.Length)
						TextBox.CaretIndex++;
					e.Handled = true;
					break;

				case (Key.Up):
					SetValueAtIndex(GetValueAtIndex(caretIndex) + 1, caretIndex);
					e.Handled = true;
					break;

				case (Key.Down):
					SetValueAtIndex(GetValueAtIndex(caretIndex) - 1, caretIndex);
					e.Handled = true;
					break;

				case (Key.Tab):
					break;

				case (Key.Back):
					SetValueAtIndex(0, caretIndex);
					e.Handled = true;
					break;

				default:
					e.Handled = true;
					break;
			}
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (e.Text.Any(x => !Char.IsDigit(x)))
				e.Handled = true;
			return;
		}

		private void Inc_Click(object sender, RoutedEventArgs e)
		{
			if (TextBox.CaretIndex <= 2 && Hours < HoursMax)
				Hours++;
			else if (TextBox.CaretIndex > 2 && Minutes < MinutesMax)
				Minutes++;
		}

		private void Dec_Click(object sender, RoutedEventArgs e)
		{
			if (TextBox.CaretIndex <= 2 && Hours > HoursMin)
				Hours--;
			else if (TextBox.CaretIndex > 2 && Minutes > MinutesMin)
				Minutes--;
		}
	}
}