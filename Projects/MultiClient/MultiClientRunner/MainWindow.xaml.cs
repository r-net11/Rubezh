using System.Windows;
using System.Linq;
using System.Diagnostics;
using System.Windows.Documents;
using System.Collections.Generic;
using MuliclientAPI;

namespace MultiClientRunner
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			MulticlientServer.Start();
		}

		List<Process> Processes = new List<Process>();

		List<AppItem> AppItems = new List<AppItem>();

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var process = Processes.LastOrDefault();
			if (process != null)
			{
				process.Kill();
				Processes.Remove(process);
			}
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			MulticlientServer.Muliclient.ShowAll();
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			MulticlientServer.Muliclient.HideAll();
		}

		private void Button_Click_5(object sender, RoutedEventArgs e)
		{
			var appItem = new AppItem();
			appItem.Initialize();
			appItem.Run();
			AppItems.Add(appItem);
		}
	}
}