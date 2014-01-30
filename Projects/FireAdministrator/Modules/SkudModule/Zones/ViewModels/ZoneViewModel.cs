using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Painters;
using XFiresecAPI;
using FiresecAPI;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class ZoneViewModel : TreeNodeViewModel<ZoneViewModel>
	{
		private VisualizationState _visualizetionState;
		public SKDZone Zone { get; private set; }

		public ZoneViewModel(SKDZone zone)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			AddToParentCommand = new RelayCommand(OnAddToParent, CanAddToParent);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			ShowParentCommand = new RelayCommand(OnShowParent, CanShowParent);

			Zone = zone;
			zone.Changed += OnChanged;
		}

		void OnChanged()
		{
			OnPropertyChanged("PresentationName");
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

		public string PresentationName
		{
			get { return Zone.PresentationName; }
		}

		public string Description
		{
			get { return Zone.Description; }
			set
			{
				Zone.Description = value;
				OnPropertyChanged("Description");
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var newDeviceViewModel = new ZoneDetailsViewModel(this);

			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				ZonesViewModel.Current.AllZones.Add(newDeviceViewModel.AddedZone);
				//Plans.Designer.Helper.BuildMap();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		public bool CanAdd()
		{
			return false;
		}

		public RelayCommand AddToParentCommand { get; private set; }
		void OnAddToParent()
		{
			Parent.AddCommand.Execute();
		}
		public bool CanAddToParent()
		{
			return ((Parent != null) && (Parent.AddCommand.CanExecute(null)));
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
			//Infrustructure.Plans.Designer.Helper.BuildMap();
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return !Zone.IsRootZone;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
		}
		bool CanShowProperties()
		{
			return false;
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

		public RelayCommand CopyCommand { get { return ZonesViewModel.Current.CopyCommand; } }
		public RelayCommand CutCommand { get { return ZonesViewModel.Current.CutCommand; } }
		public RelayCommand PasteCommand { get { return ZonesViewModel.Current.PasteCommand; } }
	}
}