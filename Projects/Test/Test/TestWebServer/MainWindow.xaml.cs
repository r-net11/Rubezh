using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestWebServer
{
	public partial class MainWindow : Window
	{
		static MainWindow Current;

		public MainWindow()
		{
			Current = this;
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ServerHost.Run();
		}

		public static void AddText(string text)
		{
			if (Current != null)
			{
				Current.Dispatcher.Invoke(new Action(() =>
				{
					Current.textBox.Text += text + "\n";
				}));
			}
		}
	}
}