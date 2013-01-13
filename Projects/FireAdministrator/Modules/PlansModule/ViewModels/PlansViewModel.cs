using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Events;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel : MenuViewPartViewModel
	{
		public ElementsViewModel ElementsViewModel { get; private set; }
		public PlansTreeViewModel PlansTreeViewModel { get; private set; }

		public PlansViewModel()
		{
			ServiceFactory.Events.GetEvent<ShowElementEvent>().Subscribe(OnShowElement);
			ServiceFactory.Events.GetEvent<FindElementEvent>().Subscribe(OnShowElementDevice);

			AddCommand = new RelayCommand(OnAdd);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);
			RemoveCommand = new RelayCommand(OnRemove, CanAddEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanAddEditRemove);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);

			DesignerCanvas = new DesignerCanvas();
			PlanDesignerViewModel = new PlanDesignerViewModel();
			PlanDesignerViewModel.DesignerCanvas = DesignerCanvas;
			DesignerCanvas.PlanDesignerViewModel = PlanDesignerViewModel;
			DesignerCanvas.Toolbox = new ToolboxViewModel(this);
			DesignerCanvas.ZoomChanged();

			InitializeCopyPaste();
			InitializeHistory();
			ElementsViewModel = new ElementsViewModel(DesignerCanvas);
			PlansTreeViewModel = new PlansTreeViewModel(this);
			CreatePages();
			_planExtensions = new List<Infrustructure.Plans.IPlanExtension<Plan>>();
			Menu = new PlansMenuViewModel(this);
		}

		public void Initialize()
		{
			using (new TimeCounter("PlansViewModel.Initialize: {0}"))
			{
				Plans = new ObservableCollection<PlanViewModel>();
				DesignerCanvas.Clear();
				foreach (var plan in FiresecManager.PlansConfiguration.Plans)
				{
					PlanDesignerViewModel.Initialize(plan);
					AddPlan(plan, null);
				}

				for (int i = 0; i < Plans.Count; i++)
				{
					Plans[i].CollapseChildren();
					Plans[i].ExpandChildren();
				}

				SelectedPlan = null;
				SelectedTabIndex = 0;
			}
		}
		private void AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
		{
			var planViewModel = new PlanViewModel(plan);
			if (parentPlanViewModel == null)
				Plans.Add(planViewModel);
			else
				parentPlanViewModel.Children.Add(planViewModel);

			foreach (var childPlan in plan.Children)
			{
				PlanDesignerViewModel.Initialize(childPlan);
				AddPlan(childPlan, planViewModel);
			}
		}

		private ObservableCollection<PlanViewModel> _plans;
		public ObservableCollection<PlanViewModel> Plans
		{
			get { return _plans; }
			set
			{
				_plans = value;
				OnPropertyChanged("Plans");
			}
		}

		private PlanViewModel _selectedPlan;
		public PlanViewModel SelectedPlan
		{
			get { return _selectedPlan; }
			set
			{
				if (SelectedPlan == value)
					return;
				using (new TimeCounter("PlansViewModel.SelectedPlan: {0}"))
				{
					_selectedPlan = value;
					OnPropertyChanged("SelectedPlan");
					DesignerCanvas.Toolbox.IsEnabled = SelectedPlan != null;
					PlanDesignerViewModel.Save();
					PlanDesignerViewModel.SelectPlan(value == null ? null : value.Plan);
					if (value != null)
						ElementsViewModel.Update();
					ResetHistory();
					DesignerCanvas.Toolbox.SetDefault();
					DesignerCanvas.DeselectAll();
				}
				Debug.WriteLine("===========================================");
			}
		}

		public PlanDesignerViewModel PlanDesignerViewModel { get; set; }

		private DesignerCanvas _designerCanvas;
		public DesignerCanvas DesignerCanvas
		{
			get { return _designerCanvas; }
			set
			{
				_designerCanvas = value;
				OnPropertyChanged("DesignerCanvas");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var planDetailsViewModel = new DesignerPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(planDetailsViewModel))
			{
				var plan = planDetailsViewModel.Plan;
				var planViewModel = new PlanViewModel(plan);
				Plans.Add(planViewModel);
				DesignerCanvas.RegisterPlan(plan);
				SelectedPlan = planViewModel;

				FiresecManager.PlansConfiguration.Plans.Add(plan);
				FiresecManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		public RelayCommand AddSubPlanCommand { get; private set; }
		private void OnAddSubPlan()
		{
			var planDetailsViewModel = new DesignerPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(planDetailsViewModel))
			{
				var plan = planDetailsViewModel.Plan;
				var planViewModel = new PlanViewModel(plan);
				SelectedPlan.Children.Add(planViewModel);
				SelectedPlan.Plan.Children.Add(plan);
				SelectedPlan.Update();
				DesignerCanvas.RegisterPlan(plan);
				SelectedPlan = planViewModel;

				FiresecManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			if (MessageBoxService.ShowConfirmation(string.Format("Вы уверены, что хотите удалить план '{0}'?", SelectedPlan.Plan.Caption)) == System.Windows.MessageBoxResult.Yes)
			{
				var selectedPlan = SelectedPlan;
				var parent = selectedPlan.Parent;
				var plan = SelectedPlan.Plan;

				DesignerCanvas.RemovePlan();
				if (parent == null)
				{
					Plans.Remove(selectedPlan);
					FiresecManager.PlansConfiguration.Plans.Remove(plan);
					foreach (var childPlanViewModel in selectedPlan.Children)
					{
						Plans.Add(childPlanViewModel);
						FiresecManager.PlansConfiguration.Plans.Add(childPlanViewModel.Plan);
						childPlanViewModel.Plan.Parent = null;
						childPlanViewModel.ResetParent();
					}
				}
				else
				{
					parent.Children.Remove(selectedPlan);
					parent.Plan.Children.Remove(plan);
					foreach (var childPlanViewModel in selectedPlan.Children)
					{
						parent.Children.Add(childPlanViewModel);
						parent.Plan.Children.Add(childPlanViewModel.Plan);
						childPlanViewModel.Plan.Parent = parent.Plan;
					}
					parent.Update();
					parent.IsExpanded = true;
				}

				FiresecManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
				SelectedPlan = Plans.FirstOrDefault();
			}
		}
		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var planDetailsViewModel = new DesignerPropertiesViewModel(SelectedPlan.Plan);
			if (DialogService.ShowModalWindow(planDetailsViewModel))
			{
				SelectedPlan.Update();
				DesignerCanvas.Update();
				PlanDesignerViewModel.Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		private bool CanAddEditRemove()
		{
			return SelectedPlan != null;
		}

		private void OnShowElement(Guid elementUID)
		{
			DesignerCanvas.Toolbox.SetDefault();
			DesignerCanvas.DeselectAll();

			foreach (var designerItem in DesignerCanvas.Items)
				if (designerItem.Element.UID == elementUID && designerItem.IsEnabled)
					designerItem.IsSelected = true;
		}
		private void OnShowElementDevice(Guid deviceUID)
		{
			var plans = new List<PlanViewModel>();
			GetAllPlans(plans, Plans);
			foreach (var plan in plans)
				foreach (var elementDevice in plan.Plan.ElementUnion)
					if (elementDevice.UID == deviceUID)
					{
						SelectedPlan = plan;
						OnShowElement(deviceUID);
						return;
					}
		}
		private void GetAllPlans(List<PlanViewModel> allPlans, IEnumerable<PlanViewModel> plans)
		{
			if (plans != null)
				foreach (var planViewModel in plans)
				{
					allPlans.Add(planViewModel);
					GetAllPlans(allPlans, planViewModel.Children);
				}
		}

		public override void OnShow()
		{
			Debug.WriteLine("===========================================");
			using (new WaitWrapper())
			using (new TimeCounter("PlansViewModel.OnShow: {0}"))
			{
				base.OnShow();
				using (new TimeCounter("PlansViewModel.UpdatePlansConfiguration: {0}"))
					FiresecManager.UpdatePlansConfiguration();
				DesignerCanvas.DeselectAll();

				if (DesignerCanvas.Toolbox != null)
					DesignerCanvas.Toolbox.AcceptKeyboard = true;
			}
			Debug.WriteLine("===========================================");
			if (SelectedPlan == null)
				SelectedPlan = Plans.FirstOrDefault();
		}
		public override void OnHide()
		{
			base.OnHide();
			if (DesignerCanvas.Toolbox != null)
				DesignerCanvas.Toolbox.AcceptKeyboard = false;
		}
	}
}