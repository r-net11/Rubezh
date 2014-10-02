using System.Windows.Controls;
using FiresecAPI.GK;
using GKModule.ViewModels;
using Infrastructure.Client.Plans;
using Infrastructure.Client.Plans.Presenter;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;

namespace GKModule.Plans.Designer
{
	class XZonePainter : BaseZonePainter<GKZone, ShowXZoneEvent>
	{
		private ZoneViewModel _zoneViewModel;

		public XZonePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_zoneViewModel = new ViewModels.ZoneViewModel(Item);
		}

		protected override GKZone CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as IElementZone;
			return element == null ? null : PlanPresenter.Cache.Get<GKZone>(element.ZoneUID);
		}
		protected override StateTooltipViewModel<GKZone> CreateToolTip()
		{
			return new ZoneTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			if (Item != null)
			{
				contextMenu.Items.Add(Helper.CreateShowInTreeItem());
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Отключить все устройства",
					"pack://application:,,,/Controls;component/Images/BTurnOff.png",
					_zoneViewModel.SetIgnoreAllCommand
				));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Снять отключения всех устройств",
					"pack://application:,,,/Controls;component/Images/BResetIgnore.png",
					_zoneViewModel.ResetIgnoreAllCommand
				));
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать связанные события",
					"pack://application:,,,/Controls;component/Images/BJournal.png",
					_zoneViewModel.ShowJournalCommand
				));
				contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			}
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new ZoneDetailsViewModel(Item);
		}
	}
}