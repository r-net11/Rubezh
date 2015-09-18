using System;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.Windows;
using Resurs.Service;

namespace Resurs.Views
{
	public partial class MainView : Window
	{
		public MainView()
		{
			InitializeComponent();
			NotifyIconService.Start(OnShow, OnClose);
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			ShowInTaskbar = true;
			Closing += MainView_Closing;
		}

		void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
			ShowInTaskbar = false;
		}

		private void OnShow(object sender, EventArgs e)
		{
			WindowState = WindowState.Normal;
			Show();
			Activate();
			ShowInTaskbar = true;
		}
		private void OnClose(object sender, EventArgs e)
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить сервер Ресурс?"))
			{
				Close();
				NotifyIconService.Stop();
				Bootstrapper.Close();
			}
		}
	}
}