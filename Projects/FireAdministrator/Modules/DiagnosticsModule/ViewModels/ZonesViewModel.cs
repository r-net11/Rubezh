using Infrastructure;
using Infrastructure.Common.Windows;
using RubezhAPI.GK;

namespace DiagnosticsModule.ViewModels
{
	public partial class ZonesViewModel : ItemsBaseViewModel<GKZone, ZoneViewModel>
	{
		public ZonesViewModel()
		{
			Menu = new ZonesMenuViewModel(this);
		}

		public override GKZone OnAdding()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(null);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				return zoneDetailsViewModel.Zone;
			}
			return null;
		}

		public override GKZone OnAddingChild()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(null);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				return zoneDetailsViewModel.Zone;
			}
			return null;
		}

		protected override void OnEdit()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(SelectedItem.Item);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				OnChanging();
				SelectedItem.Update();
			}
		}

		public override void OnShow()
		{
			base.OnShow();
		}
		public override void OnHide()
		{
			base.OnHide();
		}

		public override void OnChanging()
		{
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public override void SelectionChanged()
		{
		}
	}
}