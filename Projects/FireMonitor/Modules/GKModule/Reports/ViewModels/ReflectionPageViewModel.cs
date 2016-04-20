using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.SKDReports;
using RubezhAPI.SKD.ReportFilters;
using System.Collections.ObjectModel;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows;
using Common;
using RubezhClient;
using RubezhAPI.GK;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class ReflectionPageViewModel : FilterContainerViewModel
	{
		public ReflectionPageViewModel()
		{
			Title = "Отражения";
			Devices = new ObservableCollection<GKDevice>(GKManager.Devices.Where(x => x.DriverType == GKDriverType.GKMirror));
			SelectedDevice = Devices.FirstOrDefault();
		}

		public ObservableCollection<GKDevice> Devices { get; private set; }

		GKDevice _selectedDevice;
		public GKDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}
		public override void LoadFilter(SKDReportFilter filter)
		{
			var mirrorfilter = filter as IReportFilterReflection;
			if (mirrorfilter == null)
				return;
			if (!filter.IsDefault)
				SelectedDevice = Devices.FirstOrDefault(x => x.UID == mirrorfilter.Mirror);
			else
				SelectedDevice = Devices.FirstOrDefault();
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var mirrorfilter = filter as IReportFilterReflection;
			if (mirrorfilter == null)
				return;
			if (SelectedDevice != null)
				mirrorfilter.Mirror = SelectedDevice.UID;
		}
	}
}