using System.Windows.Controls;
using System.Linq;
using GKModule.ViewModels;
using System.Diagnostics;
using System.Collections.Generic;
using Infrastructure.Models;

namespace GKModule.Views
{
	public partial class JournalView : UserControl
	{
		public JournalView()
		{
			InitializeComponent();
		}

		bool IsManyGK
		{
			get
			{
				if ((DataContext as JournalViewModel) != null)
				{
					var dataContext = DataContext as JournalViewModel;
					return dataContext.IsManyGK;
				}
				else
				{
					var dataContext = DataContext as ArchiveViewModel;
					return dataContext.IsManyGK;
				}
			}
		}

		void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (IsManyGK)
			{
				dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "IP-адрес ГК").Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "IP-адрес ГК").Visibility = System.Windows.Visibility.Collapsed;
			}
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
			if (!IsManyGK)
			{
				if (additionalColumns.Any(x => x == JournalColumnType.GKIpAddress))
				{
					dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "IP-адрес ГК").Visibility = System.Windows.Visibility.Visible;
				}
				else
				{
					dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "IP-адрес ГК").Visibility = System.Windows.Visibility.Collapsed;
				}
			}
			if (additionalColumns.Any(x => x == JournalColumnType.SubsystemType))
			{
				dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Подсистема").Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Подсистема").Visibility = System.Windows.Visibility.Collapsed;
			}
			if (additionalColumns.Any(x => x == JournalColumnType.UserName))
			{
				dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Пользователь").Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Пользователь").Visibility = System.Windows.Visibility.Collapsed;
			}
		}
	}
}