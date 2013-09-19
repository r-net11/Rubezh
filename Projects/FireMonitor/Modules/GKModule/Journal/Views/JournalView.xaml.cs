using System.Windows.Controls;
using System.Linq;
using GKModule.ViewModels;
using System.Diagnostics;

namespace GKModule.Views
{
	public partial class JournalView : UserControl
	{
		public JournalView()
		{
			InitializeComponent();
		}

		void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			
			if((DataContext as JournalViewModel) != null)
			{
				var dataContext = DataContext as JournalViewModel;

				if (dataContext.ShowSubsystem)
					dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Подсистема").Visibility = System.Windows.Visibility.Visible;
				else
					dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Подсистема").Visibility = System.Windows.Visibility.Collapsed;

				if (!dataContext.IsManyGK)
				{
					if (dataContext.ShowIp)
						dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Ip-адрес ГК").Visibility = System.Windows.Visibility.Visible;
					else
						dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Ip-адрес ГК").Visibility = System.Windows.Visibility.Collapsed;
				}
			}
			else
			{
				var dataContext = DataContext as ArchiveViewModel;

				if (dataContext.ShowSubsystem)
					dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Подсистема").Visibility = System.Windows.Visibility.Visible;
				else
					dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Подсистема").Visibility = System.Windows.Visibility.Collapsed;

				if (!dataContext.IsManyGK)
				{
					if (dataContext.ShowIp)
						dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Ip-адрес ГК").Visibility = System.Windows.Visibility.Visible;
					else
						dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Ip-адрес ГК").Visibility = System.Windows.Visibility.Collapsed;
				}
			}
		}
	}
}