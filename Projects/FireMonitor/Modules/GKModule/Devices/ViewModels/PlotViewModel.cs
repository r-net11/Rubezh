using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace GKModule.ViewModels
{
	class PlotViewModel : DialogViewModel
	{
		public List<CurrentConsumption> CurrentConsumptions { get; set; }
		public RingCurrentConsumptions RingCurrentConsumptions { get; set; }

		public Guid DeviceUid { get; private set; }
		public Action UpdateFromDBAction { get; set; }
		bool cancelBackgroundWorker;

		public PlotViewModel(GKDevice device)
		{
			Title = "График токопотребления " + device.PresentationName;
			CurrentConsumptions = new List<CurrentConsumption>();
			DeviceUid = device.UID;
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
			cancelBackgroundWorker = true;
			GetKauMesures(StartTime, EndTime);
		}

		Thread GetKayMeasureThread;
		DispatcherTimer updateCollectionTimer;

		public void Update(object sender, EventArgs e)
		{
			var measuresResult = ClientManager.FiresecService.GetAlsMeasure(DeviceUid);
			if (measuresResult == null)
				return;
			if (measuresResult.HasError)
			{
			//    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
			//        MessageBoxService.Show(measuresResult.Error)));
			    return;
			}
			CurrentConsumptions.Add(measuresResult.Result);
		}

		void GetKauMesures(DateTime startDateTime, DateTime endDateTime)
		{
			CurrentConsumptions = new List<CurrentConsumption>();
			var measuresResult = ClientManager.FiresecService.GetCurrentConsumption(new CurrentConsumptionFilter
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
			if (UpdateFromDBAction != null)
				UpdateFromDBAction();
		}

		public override void OnClosed()
		{
			cancelBackgroundWorker = true;
		}
	}

	public class RingCurrentConsumptions : RingArray<CurrentConsumption>
	{
		private const int TOTAL_POINTS = 60;

		public RingCurrentConsumptions()
			: base(TOTAL_POINTS)
		{
		}
	}
}
