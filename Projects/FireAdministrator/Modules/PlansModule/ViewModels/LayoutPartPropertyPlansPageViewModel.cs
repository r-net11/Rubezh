using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using StrazhAPI.Models;
using StrazhAPI.Models.Layouts;
using FiresecClient;
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
			foreach (var plan in FiresecManager.PlansConfiguration.Plans)
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

		private PlanViewModel _selectedPlan;
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
		private LayoutPartPlansType _type;
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

		public override string Header
		{
			get { return "Планы"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartPlansProperties)_layoutPartPlansViewModel.Properties;
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
					list.Add(SelectedPlan.Plan.UID);
					break;
				case LayoutPartPlansType.Selected:
					list = new List<Guid>();
					AddSelected(list, Plans);
					break;
			}
			var properties = (LayoutPartPlansProperties)_layoutPartPlansViewModel.Properties;
			if (properties.Type != Type || (properties.Type != LayoutPartPlansType.All && !properties.Plans.OrderBy(item => item).SequenceEqual(list.OrderBy(item => item))))
			{
				properties.Type = Type;
				properties.Plans = list;
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