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
using Common;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing;
using ReportsModule.ViewModels;
using Controls;

namespace ReportsModule.Views
{
	/// <summary>
	/// Interaction logic for SKDReportPresenterView.xaml
	/// </summary>
	public partial class SKDReportPresenterView : UserControl
	{
		public SKDReportPresenterView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(SKDReportPresenterView_Loaded);
			currentPage.PreviewTextInput += new TextCompositionEventHandler(PreviewTextInputHandler);
			DataObject.AddPastingHandler(currentPage, PastingHandler);
		}

		private void SKDReportPresenterView_Loaded(object sender, RoutedEventArgs e)
		{
			var surface = VisualHelper.FindVisualChild<PreviewSurface>(viewer);
			if (surface != null)
			{
				var border = VisualHelper.FindVisualChild<Border>(surface);
				border.BorderThickness = new Thickness(0);
			}
		}

		private bool IsTextAllowed(string text)
		{
			return Array.TrueForAll<char>(text.ToCharArray(), (c) => char.IsDigit(c) || char.IsControl(c));
		}

		private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(e.Text);
		}
		private void PastingHandler(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(string)))
			{
				string text = (string)e.DataObject.GetData(typeof(string));
				if (!IsTextAllowed(text))
					e.CancelCommand();
			}
			else
				e.CancelCommand();
		}
	}
}
