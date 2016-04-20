using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using RubezhAPI.Models.Layouts;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace FireMonitor.Layout.ViewModels
{
	public class TimePresenterViewModel : BaseViewModel
	{
		private static DispatcherTimer _dispatcherTimer;
		static TimePresenterViewModel()
		{
			_dispatcherTimer = new DispatcherTimer();
			_dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
			_dispatcherTimer.Start();
		}

		private LayoutPartTimeProperties _properties;
		public TimePresenterViewModel(LayoutPartTimeProperties properties)
		{
			_properties = properties;

			BackgroundBrush = new SolidColorBrush(properties.BackgroundColor.ToWindowsColor());
			ForegroundBrush = new SolidColorBrush(properties.ForegroundColor.ToWindowsColor());
			BorderBrush = new SolidColorBrush(properties.BorderColor.ToWindowsColor());
			BorderThickness = properties.BorderThickness;
			FontSize = properties.FontSize;
			FontStyle = properties.FontItalic ? FontStyles.Italic : FontStyles.Normal;
			FontWeight = properties.FontBold ? FontWeights.Bold : FontWeights.Normal;
			FontFamily = new FontFamily(properties.FontFamilyName);
			HorizontalAlignment = (HorizontalAlignment)properties.HorizontalAlignment;
			VerticalAlignment = (VerticalAlignment)properties.VerticalAlignment;
			Stretch = properties.Stretch ? Stretch.Fill : Stretch.None;
			_dispatcherTimer.Tick += (s, e) => Text = DateTime.Now.ToString(_properties.Format);
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		public Brush BackgroundBrush { get; private set; }
		public Brush ForegroundBrush { get; private set; }
		public Brush BorderBrush { get; private set; }
		public double BorderThickness { get; private set; }
		public double FontSize { get; private set; }
		public FontStyle FontStyle { get; private set; }
		public FontWeight FontWeight { get; private set; }
		public FontFamily FontFamily { get; private set; }
		public HorizontalAlignment HorizontalAlignment { get; private set; }
		public VerticalAlignment VerticalAlignment { get; private set; }
		public Stretch Stretch { get; private set; }
	}
}