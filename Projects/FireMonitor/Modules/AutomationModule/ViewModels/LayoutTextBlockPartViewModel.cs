using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using RubezhAPI.Automation;
using RubezhAPI.Models.Layouts;
using System.Windows;
using System.Windows.Media;

namespace AutomationModule.ViewModels
{
	public class LayoutTextBlockPartViewModel : BaseViewModel, ILayoutPartControl
	{
		public LayoutTextBlockPartViewModel(LayoutPartTextProperties properties)
		{
			BackgroundBrush = new SolidColorBrush(properties.BackgroundColor.ToWindowsColor());
			FontStyle = properties.FontItalic ? FontStyles.Italic : FontStyles.Normal;
			FontWeight = properties.FontBold ? FontWeights.Bold : FontWeights.Normal;
			FontFamily = new FontFamily(properties.FontFamilyName);
			FontSize = properties.FontSize;
			ForegroundBrush = new SolidColorBrush(properties.ForegroundColor.ToWindowsColor());
			Text = properties.Text;
			TextAlignment = (TextAlignment)properties.TextAlignment;
			TextTrimming = properties.TextTrimming ? TextTrimming.CharacterEllipsis : TextTrimming.None;
			HorizontalAlignment = (HorizontalAlignment)properties.HorizontalAlignment;
			VerticalAlignment = (VerticalAlignment)properties.VerticalAlignment;
			TextWrapping = properties.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
		}

		private string _tetx;
		public string Text
		{
			get { return _tetx; }
			set
			{
				_tetx = value;
				OnPropertyChanged(() => Text);
			}
		}

		public HorizontalAlignment HorizontalAlignment { get; private set; }
		public VerticalAlignment VerticalAlignment { get; private set; }
		public Brush BackgroundBrush { get; private set; }
		public Brush ForegroundBrush { get; private set; }
		public FontFamily FontFamily { get; private set; }
		public double FontSize { get; private set; }
		public FontStyle FontStyle { get; private set; }
		public FontWeight FontWeight { get; private set; }
		public TextAlignment TextAlignment { get; private set; }
		public TextWrapping TextWrapping { get; private set; }
		public TextTrimming TextTrimming { get; private set; }

		#region ILayoutPartControl Members

		public object GetProperty(LayoutPartPropertyName property)
		{
			return property == LayoutPartPropertyName.Text ? Text : null;
		}

		public void SetProperty(LayoutPartPropertyName property, object value)
		{
			if (property == LayoutPartPropertyName.Text)
				Text = value.ToString();
		}

		#endregion
	}
}
