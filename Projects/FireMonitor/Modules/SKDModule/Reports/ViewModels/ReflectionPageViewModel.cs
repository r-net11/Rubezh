using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;
using SKDModule.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Common;
using FiresecClient;
using FiresecAPI.GK;

namespace SKDModule.Reports.ViewModels
{
	public class ReflectionPageViewModel : FilterContainerViewModel
	{
		public ReflectionPageViewModel()
		{
			Title = "Отражения";
			Mirror = new ObservableCollection<GKDevice>(GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_GKMirror));
			SelectedMirror = Mirror.FirstOrDefault();
		}

		public ObservableCollection<GKDevice> Mirror { get; private set; }

		GKDevice _selectedMirror;
		public GKDevice SelectedMirror
		{
			get { return _selectedMirror; }
			set
			{
				_selectedMirror = value;
				OnPropertyChanged(() => SelectedMirror);
			}
		}
		public override void LoadFilter(SKDReportFilter filter)
		{
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var mirrorfilter = filter as IReportFilterReflection;
			if (mirrorfilter == null)
				return;
			if (SelectedMirror!=null)
			mirrorfilter.Mirror = SelectedMirror.UID;
		}
	}
}

