using System.Windows;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class ArchiveView
	{
		public ArchiveView()
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
			var archiveViewModel = DataContext as ArchiveViewModel;
			_grid.Child = archiveViewModel.CellPlayerWrap;
		}
	}
}