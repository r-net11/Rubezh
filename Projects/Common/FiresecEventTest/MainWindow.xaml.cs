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
using System.Collections.ObjectModel;

namespace FiresecEventTest
{
	public partial class MainWindow : Window
	{
		public static MainWindow Current;

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			JournalItems = new ObservableCollection<string>();
			Current = this;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Runner.Run();
		}

		public ObservableCollection<string> JournalItems { get; private set; }
	}
}