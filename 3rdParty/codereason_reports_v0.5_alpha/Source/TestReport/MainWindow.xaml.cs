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
using System.IO;
using CodeReason.Reports;
using System.Data;
using System.Windows.Xps.Packaging;

namespace TestReport
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
        private bool _firstActivated = true;

        private void Window_Activated(object sender, EventArgs e)
        {
			//if (!_firstActivated) return;
			//_firstActivated = false;

            try
            {
                ReportDocument reportDocument = new ReportDocument();

				StreamReader reader = new StreamReader(new FileStream(@"..\..\Templates\T13.xaml", FileMode.Open, FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Templates\");
                reader.Close();

                ReportData data = new ReportData();

                // set constant document values
                data.ReportDocumentValues.Add("PrintDate", DateTime.Now); // print date is now

                // sample table "Ean"
                DataTable table = new DataTable("Ean");
                table.Columns.Add("Position", typeof(string));
                table.Columns.Add("Item", typeof(string));
                table.Columns.Add("EAN", typeof(string));
                table.Columns.Add("Count", typeof(int));
                Random rnd = new Random(1234);
                for (int i = 1; i <= 100; i++)
                {
                    // randomly create some articles
                    table.Rows.Add(new object[] { i, "Item " + i.ToString("0000"), "123456790123", rnd.Next(9) + 1 });
                }
                data.DataTables.Add(table);

                DateTime dateTimeStart = DateTime.Now; // start time measure here

                XpsDocument xps = reportDocument.CreateXpsDocument(data);
                documentViewer.Document = xps.GetFixedDocumentSequence();

                // show the elapsed time in window title
                Title += " - generated in " + (DateTime.Now - dateTimeStart).TotalMilliseconds + "ms";
            }
            catch (Exception ex)
            {
                // show exception
                MessageBox.Show(ex.Message + "\r\n\r\n" + ex.GetType() + "\r\n" + ex.StackTrace, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
	}
}
