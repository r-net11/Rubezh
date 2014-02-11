using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class ZoneViewModel : TreeNodeViewModel<ZoneViewModel>
	{
		private VisualizationState _visualizetionState;
		public SKDZone Zone { get; private set; }

		public ZoneViewModel(SKDZone zone)
		{
			AddCommand = new RelayCommand(OnAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);

			Zone = zone;
			zone.Changed += OnChanged;
		}

		void OnChanged()
		{
			OnPropertyChanged("Name");
		}

		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(SKDZone zone)
		{
			Zone = zone;
			OnPropertyChanged("Zone");
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
			Update();
		}

		public void Update()
		{
			OnPropertyChanged("HasChildren");
			OnPropertyChanged("IsOnPlan");

			if (Zone.PlanElementUIDs == null)
				Zone.PlanElementUIDs = new List<Guid>();
			_visualizetionState = Zone.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Zone.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}

		public string Name
		{
			get { return Zone.Name; }
		}

		public string Description
		{
			get { return Zone.Description; }
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				var zone = zoneDetailsViewModel.Zone;
				SKDManager.Zones.Add(zone);
				var zoneViewModel = new ZoneViewModel(zone);
				this.Zone.Children.Add(zone);
				this.AddChild(zoneViewModel);
				this.Update();
				SKDManager.SKDConfiguration.Update();
				ZonesViewModel.Current.AllZones.Add(zoneViewModel);
				Plans.Designer.Helper.BuildMap();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public RelayCommand AddToParentCommand { get; private set; }
		void OnAddToParent()
		{
			Parent.AddCommand.Execute();
		}
		public bool CanAddToParent()
		{
			return Parent != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var allDevices = Zone.Children;
			foreach (var device in allDevices)
			{
				SKDManager.Zones.Remove(device);
			}
			var parent = Parent;
			if (parent != null)
			{
				var index = ZonesViewModel.Current.SelectedZone.VisualIndex;
				parent.Nodes.Remove(this);
				parent.Update();

				index = Math.Min(index, parent.ChildrenCount - 1);
				foreach (var device in allDevices)
				{
					ZonesViewModel.Current.AllZones.RemoveAll(x => x.Zone.UID == device.UID);
				}
				ZonesViewModel.Current.AllZones.Remove(this);
				ZonesViewModel.Current.SelectedZone = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
			if (Zone.Parent != null)
			{
				Zone.Parent.Children.Remove(Zone);
			}
			Plans.Designer.Helper.BuildMap();
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return !Zone.IsRootZone;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(this.Zone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				this.Zone = zoneDetailsViewModel.Zone;
				Update(this.Zone);
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		public bool CanEdit()
		{
			return !Zone.IsRootZone;
		}

		public bool IsOnPlan
		{
			get { return Zone.PlanElementUIDs.Count > 0; }
		}
		public bool ShowOnPlan
		{
			get { return true; }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Zone.PlanElementUIDs.Count > 0)
				ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(Zone.PlanElementUIDs);
		}

		public RelayCommand ShowParentCommand { get; private set; }
		void OnShowParent()
		{
			ServiceFactoryBase.Events.GetEvent<ShowSKDZoneEvent>().Publish(Zone.Parent.UID);
		}
		bool CanShowParent()
		{
			return Zone.Parent != null;
		}

		public bool IsBold { get; set; }
	}
}