using System.Collections.Generic;
using FiresecAPI.SKD;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows;
using SKDModule.ViewModels;

namespace SKDModule
{
	public class CardReportProvider //: FilteredSKDReportProvider<CardReportFilter>
	{
		List<CardReportFilter> CardReportFilters;

		public CardReportProvider(int index)
			//: base("Фильтр по пропускам", "Фильтр по пропускам", index)
		{
			//Filter = new CardReportFilter();
			CardReportFilters = new List<CardReportFilter>();
			CardReportFilters.Add(new CardReportFilter { Name = "По умолчанию" });
			CardReportFilters.Add(new CardReportFilter { Name = "Фильтр 1" });
			CardReportFilters.Add(new CardReportFilter { Name = "Фильтр 2" });
		}

		//public override bool ChangeFilter()
		//{
		//    var cardReportFilterDetailsViewModel = new CardReportFilterDetailsViewModel(CardReportFilters);
		//    if (DialogService.ShowModalWindow(cardReportFilterDetailsViewModel))
		//    {
		//        Filter = cardReportFilterDetailsViewModel.SelectedCardReportFilter;
		//        return true;
		//    }
		//    return false;
		//}
	}
}