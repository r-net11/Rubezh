using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Common;

namespace Resurs.ViewModels
{
	public class FilterTariffsViewModel :BaseViewModel
	{
		public FilterTariffsViewModel(Filter filter)
		{
			FilterTariffViewModel = new ObservableCollection<FilterTariffViewModel>();
			DbCache.Tariffs.ForEach(x => FilterTariffViewModel.Add(new FilterTariffViewModel(x)));
			FilterTariffViewModel.ForEach(x =>
				{
					if (filter.TariffUIDs.Contains(x.Tariff.UID))
						x.IsChecked = true;
				});
		}

		public ObservableCollection<FilterTariffViewModel> FilterTariffViewModel { get; set; }

		public List<Guid?> GetTariffUIDs()
		{
			List<Guid?> TariffUIDs = new List<Guid?>();
			foreach (var tariff in FilterTariffViewModel)
			{
				if (tariff.IsChecked)
					TariffUIDs.Add(tariff.Tariff.UID);
			}
			return TariffUIDs;
		}
	}
}