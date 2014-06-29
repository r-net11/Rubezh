using System.Windows.Controls;
using System.Windows.Media;
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
	class XGuardZonePainter : BaseZonePainter<XGuardZone, ShowXGuardZoneEvent>
	{
		private GuardZoneViewModel _guardZoneViewModel;

		public XGuardZonePainter(PresenterItem presenterItem)
			: base(presenterItem)
		{
			if (Item != null)
				_guardZoneViewModel = new ViewModels.GuardZoneViewModel(Item);
		}

		protected override XGuardZone CreateItem(PresenterItem presenterItem)
		{
			var element = presenterItem.Element as IElementZone;
			return element == null ? null : PlanPresenter.Cache.Get<XGuardZone>(element.ZoneUID);
		}
		protected override StateTooltipViewModel<XGuardZone> CreateToolTip()
		{
			return new GuardZoneTooltipViewModel(Item);
		}
		protected override ContextMenu CreateContextMenu()
		{
			var contextMenu = new ContextMenu();
			if (Item != null)
			{
				contextMenu.Items.Add(Helper.CreateShowInTreeItem());
				contextMenu.Items.Add(UIHelper.BuildMenuItem(
					"Показать связанные события",
					"pack://application:,,,/Controls;component/Images/BJournal.png",
					_guardZoneViewModel.ShowJournalCommand
				));
				contextMenu.Items.Add(Helper.CreateShowPropertiesItem());
			}
			return contextMenu;
		}
		protected override WindowBaseViewModel CreatePropertiesViewModel()
		{
			return new GuardZoneDetailsViewModel(Item);
		}

		protected override Color GetStateColor()
		{
			return Item.State.StateClass == XStateClass.Norm ? Colors.Brown : base.GetStateColor();
		}
	}
}