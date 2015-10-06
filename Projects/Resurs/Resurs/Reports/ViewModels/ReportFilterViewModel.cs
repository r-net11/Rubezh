using Infrastructure.Common.Windows.ViewModels;
using Resurs.Reports;
using ResursAPI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Resurs.ViewModels
{
	public class ReportFilterViewModel : SaveCancelDialogViewModel
	{
		public ReportFilter Filter
		{
			get
			{
				var filter = new ReportFilter();
				filter.StartDate = StartDate;
				filter.EndDate = EndDate;
				filter.Device = SelectedDevice;
				return filter;
			}
		}
		public ReportFilterViewModel()
		{
			Title = "Настройки отчета";
			StartDate = DateTime.Today;
			EndDate = DateTime.Today;
			Devices = ResursDAL.DBCash.RootDevice.Children;
			SelectedDevice = Devices.FirstOrDefault();
		}
		private DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged(() => StartDate);
			}
		}
		private DateTime _endDate;
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = value;
				OnPropertyChanged(() => EndDate);
			}
		}
		public List<Device> Devices { get; set; }
		private Device _selectedDevice;
		public Device SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}
	}
}