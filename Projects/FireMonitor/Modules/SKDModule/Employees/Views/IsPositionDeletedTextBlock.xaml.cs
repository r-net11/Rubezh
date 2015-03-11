using System.Windows;
using System.Windows.Controls;

namespace SKDModule.Views
{
	public partial class IsPositionDeletedTextBlock : UserControl
	{
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(IsPositionDeletedTextBlock),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTextPropertyChanged)));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		private static void OnTextPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			IsPositionDeletedTextBlock carthotequeTextBlock = dp as IsPositionDeletedTextBlock;
			if (carthotequeTextBlock != null)
				carthotequeTextBlock._textBlock.Text = carthotequeTextBlock.Text;
		}

		public IsPositionDeletedTextBlock()
		{
			InitializeComponent();
		}
	}
}
