using Common;
using Controls;
using DevExpress.Xpf.Printing.Native;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Resurs.Views
{
	/// <summary>
	/// Interaction logic for ReportsView.xaml
	/// </summary>
	public partial class ReportsView : UserControl
	{
		public ReportsView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(ReportsView_Loaded);
			currentPage.PreviewTextInput += new TextCompositionEventHandler(PreviewTextInputHandler);
			DataObject.AddPastingHandler(currentPage, PastingHandler);
		}
		void ReportsView_Loaded(object sender, RoutedEventArgs e)
		{
			var surface = VisualHelper.FindVisualChild<PreviewSurface>(viewer);
			if (surface != null)
			{
				var border = VisualHelper.FindVisualChild<Border>(surface);
				border.BorderThickness = new Thickness(0);
			}
		}
		bool IsTextAllowed(string text)
		{
			return Array.TrueForAll<char>(text.ToCharArray(), (c) => char.IsDigit(c) || char.IsControl(c));
		}
		void PreviewTextInputHandler(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(e.Text);
		}
		void PastingHandler(object sender, DataObjectPastingEventArgs e)
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