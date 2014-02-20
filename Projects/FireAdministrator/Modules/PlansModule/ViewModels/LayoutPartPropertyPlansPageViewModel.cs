using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Microsoft.Win32;
using FiresecAPI.Models;
using FiresecClient;

namespace PlansModule.ViewModels
{
	public class LayoutPartPropertyPlansPageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartPlansViewModel _layoutPartPlansViewModel;

		public LayoutPartPropertyPlansPageViewModel(LayoutPartPlansViewModel layoutPartPlansViewModel)
		{
			_layoutPartPlansViewModel = layoutPartPlansViewModel;
			Types = new ObservableCollection<LayoutPartPlansType>(Enum.GetValues(typeof(LayoutPartPlansType)).Cast<LayoutPartPlansType>());
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.Plans)
				AddPlan(plan, null);
			CopyProperties();
			UpdateLayoutPart();
		}

		private void AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
		{
			var planViewModel = new PlanViewModel(plan);
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


		private ObservableCollection<PlanViewModel> _plans;
		public ObservableCollection<PlanViewModel> Plans
		{
			get { return _plans; }
			set
			{
				_plans = value;
				OnPropertyChanged(() => Plans);
			}
		}

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
			//SelectedPlan = FiresecManager.SystemConfiguration.Plans.FirstOrDefault(item => item.UID == properties.SourceUID);
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartPlansProperties)_layoutPartPlansViewModel.Properties;
			if (properties.Type != Type)
			{
				properties.Type = Type;
				//SelectedPlan = FiresecManager.SystemConfiguration.Plans.FirstOrDefault(item => item.UID == properties.SourceUID);
				UpdateLayoutPart();
				return true;
			}
			return false;
		}

		private void UpdateLayoutPart()
		{
			var properties = (LayoutPartPlansProperties)_layoutPartPlansViewModel.Properties;
			//_layoutPartPlansViewModel.PlansTitle = SelectedPlan == null ? _layoutPartPlansViewModel.Title : SelectedPlan.Caption;
		}
	}
}