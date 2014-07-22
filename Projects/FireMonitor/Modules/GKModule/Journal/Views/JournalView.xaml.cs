using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Common;
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
			try
			{
				List<XJournalColumnType> additionalColumns;
				if ((DataContext as JournalViewModel) != null)
				{
					var dataContext = DataContext as JournalViewModel;
					if (dataContext == null)
						return;
					additionalColumns = dataContext.AdditionalColumns;
				}
				else
				{
					var dataContext = DataContext as ArchiveViewModel;
					if (dataContext == null)
						return;
					additionalColumns = dataContext.AdditionalColumns;
				}
				var ipColumn = dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "IP-адрес ГК");
				if (ipColumn == null)
				{
					Logger.Error("JournalView ipColumn == null");
					return;
				}

				var subsystemColumn = dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Подсистема");
				if (subsystemColumn == null)
				{
					Logger.Error("JournalView subsystemColumn == null");
					return;
				}

				var userColumn = dataGrid.Columns.FirstOrDefault(x => x.Header.ToString() == "Пользователь");
				if (userColumn == null)
				{
					Logger.Error("JournalView userColumn == null");
					return;
				}

				if (additionalColumns.Any(x => x == XJournalColumnType.GKIpAddress))
					ipColumn.Visibility = System.Windows.Visibility.Visible;
				else
					ipColumn.Visibility = System.Windows.Visibility.Collapsed;

				if (additionalColumns.Any(x => x == XJournalColumnType.SubsystemType))
					subsystemColumn.Visibility = System.Windows.Visibility.Visible;
				else
					subsystemColumn.Visibility = System.Windows.Visibility.Collapsed;

				if (additionalColumns.Any(x => x == XJournalColumnType.UserName))
					userColumn.Visibility = System.Windows.Visibility.Visible;
				else
					userColumn.Visibility = System.Windows.Visibility.Collapsed;
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "JournalView.CheckBox_AdditionalColumns_Checked");
			}
		}

		private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (dataGrid != null && dataGrid.SelectedItem != null)
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
		}
	}
}