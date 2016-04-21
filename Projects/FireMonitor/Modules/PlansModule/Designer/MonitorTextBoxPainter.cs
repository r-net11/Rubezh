using Infrastructure.Plans;
using Infrastructure.Plans.Painters;
using Infrastructure.Plans.Presenter;
using RubezhAPI.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlansModule.Designer
{
	public class MonitorTextBoxPainter : TextBoxPainter
	{
		MonitorTextBoxPresenterItem _monitorTextBoxPresenterItem;

		public MonitorTextBoxPainter(MonitorTextBoxPresenterItem presenterItem)
			: base(presenterItem.DesignerCanvas, presenterItem.Element)
		{
			_monitorTextBoxPresenterItem = presenterItem;
			var elementTextBox = presenterItem.ElementTextBox;
		}
	}

	public class MonitorTextBoxPresenterItem : PresenterItem
	{
		public ElementTextBox ElementTextBox { get; private set; }

		public MonitorTextBoxPresenterItem(ElementTextBox element)
			: base(element)
		{
			var elementTextBox = ElementTextBox = element;

			var textBox = new TextBox();
			textBox.Style = null;
			textBox.BorderBrush = new SolidColorBrush(elementTextBox.BorderColor.ToWindowsColor());
			textBox.BorderThickness = new Thickness(elementTextBox.BorderThickness);
			textBox.Width = elementTextBox.Width;
			textBox.Height = elementTextBox.Height;
			textBox.Text = elementTextBox.Text;
			textBox.FontSize = elementTextBox.FontSize;
			textBox.FontFamily = new FontFamily(elementTextBox.FontFamilyName);
			if (elementTextBox.FontItalic)
				textBox.FontStyle = FontStyles.Italic;
			if (elementTextBox.FontBold)
				textBox.FontWeight = FontWeights.Bold;
			if (elementTextBox.WordWrap)
				textBox.TextWrapping = TextWrapping.Wrap;
			textBox.Background = new SolidColorBrush(elementTextBox.BackgroundColor.ToWindowsColor());
			textBox.Foreground = new SolidColorBrush(elementTextBox.ForegroundColor.ToWindowsColor());
			Canvas.SetLeft(textBox, elementTextBox.Left);
			Canvas.SetTop(textBox, elementTextBox.Top);
			WPFControl = textBox;
		}
	}
}