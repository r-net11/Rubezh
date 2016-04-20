using System.Windows.Controls;
using RubezhAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrustructure.Plans.Presenter;

namespace Infrastructure.Client.Plans.Presenter
{
	public interface IBasePainter<T, TShowEvent>
			where T : IStateProvider
	{
		T CreateItem(PresenterItem presenterItem);
		StateTooltipViewModel<T> CreateToolTip();
		ContextMenu CreateContextMenu();
		WindowBaseViewModel CreatePropertiesViewModel();
		RelayCommand ShowInTreeCommand { get; set; }
		RelayCommand ShowPropertiesCommand { get; set; }
		bool IsPoint { get; }
	}
}
