using Common;
using Controls.Menu.ViewModels;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using RubezhAPI.GK;
using RubezhAPI.Hierarchy;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DiagnosticsModule.ViewModels
{
	public partial class ZonesViewModel : ItemsBaseViewModel<GKZone, ZoneViewModel>
	{
		public BaseViewModel Menu { get; protected set; }
		public ObservableCollection<MenuButtonViewModel> ZonesMenu { get; private set; }

		public ZonesViewModel()
		{
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