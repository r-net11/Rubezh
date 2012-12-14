using System.Windows;
using System.Linq;
using System.Diagnostics;
using System.Windows.Documents;
using System.Collections.Generic;
using MuliclientAPI;
using System.Threading;
using System;

namespace MultiClientRunner
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			AppItemsViewModels = new AppItemsViewModels();
			MulticlientServer.Start();
            DataContext = AppItemsViewModels;

            MulticlientDatas = new List<MulticlientData>();
            var multiclientData1 = new MulticlientData()
            {
				Id = "0",
                Name = "Server1",
                RemoteAddress = "localhost",
                RemotePort = 0,
                Login = "adm",
                Password = ""
            };
            MulticlientDatas.Add(multiclientData1);
            var multiclientData2 = new MulticlientData()
            {
				Id = "1",
                Name = "Server2",
                RemoteAddress = "localhost",
                RemotePort = 0,
                Login = "adm",
                Password = ""
            };
			MulticlientDatas.Add(multiclientData2);
			//var multiclientData3 = new MulticlientData()
			//{
			//    Id = "2",
			//    Name = "Server3",
			//    RemoteAddress = "localhost",
			//    RemotePort = 0,
			//    Login = "adm",
			//    Password = ""
			//};
			//MulticlientDatas.Add(multiclientData3);

			MulticlientServer.MulticlientDatas = MulticlientDatas;
		}

        public AppItemsViewModels AppItemsViewModels { get; private set; }
        List<MulticlientData> MulticlientDatas;

		private void OnKillAll(object sender, RoutedEventArgs e)
		{
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName == "FireMonitor")
                {
                    process.Kill();
                }
            }
		}

        int clientId = 0;

		private void OnAdd(object sender, RoutedEventArgs e)
		{
            foreach (var multiclientData in MulticlientDatas)
            {
				clientId++;
                var appItem = new AppItem();
                appItem.Run(multiclientData, clientId.ToString());
                AppItemsViewModels.AppItems.Add(appItem);
				Thread.Sleep(TimeSpan.FromSeconds(5));
            }
		}

        private void OnTest(object sender, RoutedEventArgs e)
		{
            MessageBox.Show(AppItemsViewModels.SelectedAppItem.GetWindowSize().Left.ToString());
        }
	}
}