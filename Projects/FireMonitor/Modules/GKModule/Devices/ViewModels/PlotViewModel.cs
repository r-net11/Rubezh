using System;
using System.Collections.Generic;
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
			GetKauMeasuresCommand = new RelayCommand<List<object>>(OnGetKauMesures);
		}

		public RelayCommand<List<object>> GetKauMeasuresCommand { get; private set; }
		public void OnGetKauMesures(List<object> objects)
		{
			CurrentConsumptions = new List<CurrentConsumption>();
			if (objects == null || objects.Count != 2)
				return;
			var measuresResult = FiresecManager.FiresecService.GetCurrentConsumption(new CurrentConsumptionFilter
				{
					AlsUID = DeviceUid,
					StartDateTime = (DateTime)objects[0],
					EndDateTime = (DateTime)objects[1]
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
