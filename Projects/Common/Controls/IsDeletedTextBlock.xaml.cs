using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	/// <summary>
	/// Логика взаимодействия для CarthotequeTextBlock.xaml
	/// </summary>
	public partial class IsDeletedTextBlock : UserControl
	{
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(IsDeletedTextBlock),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTextPropertyChanged)));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		private static void OnTextPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			IsDeletedTextBlock carthotequeTextBlock = dp as IsDeletedTextBlock;
			if (carthotequeTextBlock != null)
				carthotequeTextBlock._textBlock.Text = carthotequeTextBlock.Text;
		}

		public IsDeletedTextBlock()
		{
			InitializeComponent();
		}
	}
}
