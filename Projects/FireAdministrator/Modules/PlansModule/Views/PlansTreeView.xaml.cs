using System.Windows;
using System.Windows.Controls;

namespace PlansModule.Views
{
	public partial class PlansTreeView : UserControl
	{
		public PlansTreeView()
		{
			DataContextChanged += new DependencyPropertyChangedEventHandler(PlansTreeView_DataContextChanged);
			InitializeComponent();
		}

		void PlansTreeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == null && e.OldValue != null)
				DataContext = e.OldValue;
		}
	}
}