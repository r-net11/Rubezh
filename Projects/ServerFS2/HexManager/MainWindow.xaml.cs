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
using System.Windows.Forms;

namespace HexManager
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button1_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog();
			System.Windows.Forms.DialogResult result = dialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				var folderName = dialog.SelectedPath;
				HexZipHelper.Zip(folderName);
			}
		}

		private void Button2_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Пакет обновления (*.HXC)|*.HXC|Открытый пакет обновления (*.FSCF)|*.FSCF|All files (*.*)|*.*"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				var fileName = openFileDialog.FileName;
			}
		}
	}
}