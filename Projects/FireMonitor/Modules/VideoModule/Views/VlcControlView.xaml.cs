using System;
using System.Windows;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class VlcControlView
	{
		public VlcControlView()
		{
			InitializeComponent();

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var viewModel = DataContext as VlcControlViewModel;
			if (viewModel == null)
				return;
			viewModel.OnStart -= OnStart;
			viewModel.OnStop -= OnStop;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var viewModel = DataContext as VlcControlViewModel;
			if (viewModel == null)
				return;
			viewModel.OnStart += OnStart;
			viewModel.OnStop += OnStop;
		}

		private void OnStart(object sender, EventArgs eventArgs)
		{
			videoCellControl.MediaPlayer.Play();
		}

		private void OnStop(object sender, EventArgs eventArgs)
		{
			videoCellControl.MediaPlayer.Stop();
		}
	}
}
