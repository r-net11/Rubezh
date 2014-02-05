using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using GKModule.ViewModels;
using Infrastructure.Models;

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
			CheckBox_AdditionalColumns_Checked(sender, e);
		}
		
		void CheckBox_AdditionalColumns_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			List<JournalColumnType> additionalColumns;
			if ((DataContext as JournalViewModel) != null)
			{
				var dataContext = DataContext as JournalViewModel;
				additionalColumns = dataContext.AdditionalColumns;
			}
			else
			{
				var dataContext = DataContext as ArchiveViewModel;
				additionalColumns = dataContext.AdditionalColumns;
			}
			var ipColumn  = dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "IP-адрес ГК");
			var subsystemColumn = dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Подсистема");
			var userColumn = dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Пользователь");

			if (additionalColumns.Any(x => x == JournalColumnType.GKIpAddress))
				ipColumn.Visibility = System.Windows.Visibility.Visible;
			else
				ipColumn.Visibility = System.Windows.Visibility.Collapsed;
		
			if (additionalColumns.Any(x => x == JournalColumnType.SubsystemType))
				subsystemColumn.Visibility = System.Windows.Visibility.Visible;
			else
				subsystemColumn.Visibility = System.Windows.Visibility.Collapsed;
			
			if (additionalColumns.Any(x => x == JournalColumnType.UserName))
				userColumn.Visibility = System.Windows.Visibility.Visible;
			else
				userColumn.Visibility = System.Windows.Visibility.Collapsed;
		}
	}
}