using System;
using System.Collections.Generic;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	class PlotViewModel : DialogViewModel
	{
		public List<CurrentConsumption> CurrentConsumptions { get; set; }
		Guid DeviceUid { get; set; }
		public Action PlotViewUpdateAction { get; set; }

		public PlotViewModel(Guid deviceUid)
		{
			DeviceUid = deviceUid;
			CurrentConsumptions = new List<CurrentConsumption>();
			GetKauMeasuresCommand = new RelayCommand(OnGetKauMesures);
			StartTime = DateTime.Now.Date;
			EndTime = DateTime.Now.Date;
		}

		private DateTime _startTime;
		public DateTime StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				OnPropertyChanged(() => StartTime);
			}
		}

		private DateTime _endTime;
		public DateTime EndTime
		{
			get { return _endTime; }
			set
			{
				_endTime = value;
				OnPropertyChanged(() => EndTime);
			}
		}

		public RelayCommand GetKauMeasuresCommand { get; private set; }
		void OnGetKauMesures()
		{
			GetKauMesures(StartTime, EndTime);
		}

		public RelayCommand GetKauMeasuresOnlineCommand { get; private set; }
		void OnGetKauMeasuresOnline()
		{
			
		}

		void GetKauMesures(DateTime startDateTime, DateTime endDateTime)
		{
			CurrentConsumptions = new List<CurrentConsumption>();
			var measuresResult = FiresecManager.FiresecService.GetCurrentConsumption(new CurrentConsumptionFilter
			{
				AlsUID = DeviceUid,
				StartDateTime = startDateTime,
				EndDateTime = endDateTime
			});
			if (measuresResult == null)
				return;
			if (measuresResult.HasError)
				MessageBoxService.Show(measuresResult.Error);
			else
			{
				foreach (var measure in measuresResult.Result)
				{
					var measureTime = measure.DateTime;
					var measureValue = measure.Current;
					CurrentConsumptions.Add(new CurrentConsumption { DateTime = measureTime, Current = measureValue });
				}
			}
			if (PlotViewUpdateAction != null)
				PlotViewUpdateAction();
		}
	}
}
