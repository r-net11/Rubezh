using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Controls.TreeList;
using StrazhAPI.SKD;
using SKDModule.ViewModels;

namespace SKDModule.Views
{
	public partial class HolidaysView : UserControl, IWithDeletedView
	{
		public HolidaysView()
		{
			InitializeComponent();
			_changeIsDeletedViewSubscriber = new ChangeIsDeletedViewSubscriber(this);
		}

		ChangeIsDeletedViewSubscriber _changeIsDeletedViewSubscriber;

		public TreeList TreeList
		{
			get { return _treeList; }
			set { _treeList = value; }
		}

		private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			var deletationType = (DataContext as HolidaysViewModel).IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active;
			_changeIsDeletedViewSubscriber = new ChangeIsDeletedViewSubscriber(this, deletationType);
		}

		private void DatePicker_CalendarOpened(object sender, System.Windows.RoutedEventArgs e)
		{
			var datepicker = sender as DatePicker;
			if (datepicker == null)
				return;
			var popup = (Popup)datepicker.Template.FindName("PART_Popup", datepicker);
			var cal = (Calendar)popup.Child;
			cal.DisplayMode = CalendarMode.Decade;
			cal.DisplayModeChanged -= CalOnDisplayModeChanged;
			cal.DisplayModeChanged += CalOnDisplayModeChanged;
			cal.Unloaded -= CalOnUnloaded;
			cal.Unloaded += CalOnUnloaded;
		}

		private void CalOnDisplayModeChanged(object sender, CalendarModeChangedEventArgs calendarModeChangedEventArgs)
		{
			var cal = sender as Calendar;
			if (cal == null)
				return;
			cal.SelectedDate = new DateTime(cal.DisplayDate.Year, 1, 1);
			DatePicker.SelectedDate = cal.SelectedDate;
			DatePicker.IsDropDownOpen = false;
		}

		private void CalOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var cal = sender as Calendar;
			if (cal == null)
				return;
			cal.DisplayModeChanged -= CalOnDisplayModeChanged;
			cal.Unloaded -= CalOnUnloaded;
		}
	}
}