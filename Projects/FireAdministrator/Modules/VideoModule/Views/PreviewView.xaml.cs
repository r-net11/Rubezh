using System;
using System.Windows;
using System.Windows.Controls;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class PreviewView : UserControl
	{
		public PreviewView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			_grid.Child = new UIElement();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var previewViewModel = DataContext as PreviewViewModel;
			_grid.Child = previewViewModel.CellPlayerWrap;
		}
	}
}
