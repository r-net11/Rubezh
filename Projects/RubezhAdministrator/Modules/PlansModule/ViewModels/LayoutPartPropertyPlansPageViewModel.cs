using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using Infrastructure.Common.Services.Layout;

namespace PlansModule.ViewModels
{
	public class LayoutPartPropertyPlansPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartPlansViewModel _layoutPartPlansViewModel;
		private Dictionary<Guid, PlanViewModel> _map;

		public LayoutPartPropertyPlansPageViewModel(LayoutPartPlansViewModel layoutPartPlansViewModel)
		{
			_layoutPartPlansViewModel = layoutPartPlansViewModel;
			_map = new Dictionary<Guid, PlanViewModel>();
			Types = new ObservableCollection<LayoutPartPlansType>(Enum.GetValues(typeof(LayoutPartPlansType)).Cast<LayoutPartPlansType>());
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in ClientManager.PlansConfiguration.Plans)
				AddPlan(plan, null);
			Plans.ForEach(item => item.IsChecked = false);
		}

		private void AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
		{
			var planViewModel = new PlanViewModel(plan);
			_map.Add(plan.UID, planViewModel);
			if (parentPlanViewModel == null)
				Plans.Add(planViewModel);
			else
				parentPlanViewModel.AddChild(planViewModel);
			foreach (var childPlan in plan.Children)
				AddPlan(childPlan, planViewModel);
		}

		public bool IsTreeEnabled
		{
			get { return Type != LayoutPartPlansType.All; }
		}
		public bool ShowCheckboxes
		{
			get { return Type == LayoutPartPlansType.Selected; }
		}

		public ObservableCollection<PlanViewModel> Plans { get; private set; }

		PlanViewModel _selectedPlan;
		public PlanViewModel SelectedPlan
		{
			get { return _selectedPlan; }
			set
			{
				_selectedPlan = value;
				OnPropertyChanged(() => SelectedPlan);
			}
		}

		public ObservableCollection<LayoutPartPlansType> Types { get; private set; }

		LayoutPartPlansType _type;
		public LayoutPartPlansType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				OnPropertyChanged(() => Type);
				OnPropertyChanged(() => IsTreeEnabled);
				OnPropertyChanged(() => ShowCheckboxes);
				if (!IsTreeEnabled)
					SelectedPlan = null;
			}
		}
		bool _showZoomSliders;
		public bool ShowZoomSliders
		{
			get { return _showZoomSliders; }
			set
			{
				_showZoomSliders = value;
				OnPropertyChanged(() => ShowZoomSliders); 
			}
		}
		double _deviceZoom;
		public double DeviceZoom
		{
			get { return _deviceZoom; }
			set
			{
				_deviceZoom = value;
				OnPropertyChanged(() => DeviceZoom);
			}
		}
		bool _allowChangePlanZoom;
		public bool AllowChangePlanZoom
		{
			get { return _allowChangePlanZoom; }
			set
			{
				_allowChangePlanZoom = value;
				OnPropertyChanged(() => AllowChangePlanZoom);
			}
		}

		public override string Header
		{
			get { return "Планы"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartPlansProperties)_layoutPartPlansViewModel.Properties;
			ShowZoomSliders = properties.ShowZoomSliders;
			DeviceZoom = properties.DeviceZoom;
			AllowChangePlanZoom = properties.AllowChangePlanZoom;
			Type = properties.Type;
			switch (Type)
			{
				case LayoutPartPlansType.All:
					break;
				case LayoutPartPlansType.Single:
					if (properties.Plans.Count > 0 && _map.ContainsKey(properties.Plans[0]))
						SelectedPlan = _map[properties.Plans[0]];
					break;
				case LayoutPartPlansType.Selected:
					properties.Plans.ForEach(item =>
					{
						if (_map.ContainsKey(item))
							_map[item].IsChecked = true;
					});
					break;
			}
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			List<Guid> list = null;
			switch (Type)
			{
				case LayoutPartPlansType.All:
					break;
				case LayoutPartPlansType.Single:
					list = new List<Guid>();
					if (SelectedPlan!= null)
					list.Add(SelectedPlan.Plan.UID);
					break;
				case LayoutPartPlansType.Selected:
					list = new List<Guid>();
					AddSelected(list, Plans);
					break;
			}
			var properties = (LayoutPartPlansProperties)_layoutPartPlansViewModel.Properties;
			var hasChanges = properties.Type != Type 
				|| properties.ShowZoomSliders != ShowZoomSliders 
				|| properties.DeviceZoom != DeviceZoom
				||properties.AllowChangePlanZoom != AllowChangePlanZoom;
            if (hasChanges || (properties.Type != LayoutPartPlansType.All && !properties.Plans.OrderBy(item => item).SequenceEqual(list.OrderBy(item => item))))
			{
				properties.Type = Type;
				properties.Plans = list;
				properties.ShowZoomSliders = ShowZoomSliders;
				properties.DeviceZoom = DeviceZoom;
				properties.AllowChangePlanZoom = AllowChangePlanZoom;
				_layoutPartPlansViewModel.UpdateLayoutPart();
				return true;
			}
			return false;
		}
		private void AddSelected(List<Guid> list, IEnumerable<PlanViewModel> plans)
		{
			foreach (var plan in plans)
			{
				if (plan.IsChecked)
					list.Add(plan.Plan.UID);
				AddSelected(list, plan.Children);
			}
		}
	}
}