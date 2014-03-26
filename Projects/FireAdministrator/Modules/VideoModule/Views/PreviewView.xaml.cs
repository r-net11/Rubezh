using System;
using System.Windows;
using System.Windows.Controls;

namespace VideoModule.Views
{
	/// <summary>
	/// Логика взаимодействия для VideoPreview.xaml
	/// </summary>
	public partial class PreviewView : UserControl
	{
		public static PreviewView Current { get; private set; }
		public PreviewView()
		{
			InitializeComponent();
			Current = this;
			//Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			throw new NotImplementedException();
		}
	}
}
