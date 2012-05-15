using System;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Designer;
using PlansModule.Events;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel : RegionViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
		public ElementsViewModel ElementsViewModel { get; private set; }

		public PlansViewModel()
		{
			ServiceFactory.Events.GetEvent<ShowElementEvent>().Subscribe(OnShowElement);
			ServiceFactory.Events.GetEvent<ShowElementDeviceEvent>().Subscribe(OnShowElementDevice);

			AddCommand = new RelayCommand(OnAdd);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);
			RemoveCommand = new RelayCommand(OnRemove, CanAddEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanAddEditRemove);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);

			DesignerCanvas = new DesignerCanvas();
			PlanDesignerViewModel = new PlanDesignerViewModel();
			PlanDesignerViewModel.DesignerCanvas = DesignerCanvas;

			InitializeCopyPaste();
			InitializeHistory();
			ElementsViewModel = new ElementsViewModel(DesignerCanvas);
			DevicesViewModel = new DevicesViewModel();
		}

		public void Initialize()
		{
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.Plans)
			{
				AddPlan(plan, null);
			}

			for (int i = 0; i < Plans.Count; i++)
			{
				CollapseChild(Plans[i]);
				ExpandChild(Plans[i]);
			}

			SelectedPlan = null;
			if (Plans.Count > 0)
			{
				SelectedPlan = Plans[0];
			}
		}

		PlanViewModel AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
		{
			var planViewModel = new PlanViewModel(plan, Plans);
			planViewModel.Parent = parentPlanViewModel;

			var indexOf = Plans.IndexOf(parentPlanViewModel);
			Plans.Insert(indexOf + 1, planViewModel);

			foreach (var childPlan in plan.Children)
			{
				var childPlanViewModel = AddPlan(childPlan, planViewModel);
				planViewModel.Children.Add(childPlanViewModel);
			}

			return planViewModel;
		}

		void CollapseChild(PlanViewModel parentPlanViewModel)
		{
			parentPlanViewModel.IsExpanded = false;
			foreach (var planViewModel in parentPlanViewModel.Children)
			{
				CollapseChild(planViewModel);
			}
		}

		void ExpandChild(PlanViewModel parentPlanViewModel)
		{
			parentPlanViewModel.IsExpanded = true;
			foreach (var planViewModel in parentPlanViewModel.Children)
			{
				ExpandChild(planViewModel);
			}
		}

		ObservableCollection<PlanViewModel> _plans;
		public ObservableCollection<PlanViewModel> Plans
		{
			get { return _plans; }
			set
			{
				_plans = value;
				OnPropertyChanged("Plans");
			}
		}

		PlanViewModel _selectedPlan;
		public PlanViewModel SelectedPlan
		{
			get { return _selectedPlan; }
			set
			{
				if (_selectedPlan == value)
					return;
				_selectedPlan = value;
				OnPropertyChanged("SelectedPlan");

				PlanDesignerViewModel.Save();

				if (value != null)
				{
					PlanDesignerViewModel.ChangeZoom(1);
					PlanDesignerViewModel.Initialize(value.Plan);
					//PlanDesignerViewModel.UpdateDeviceInZones();
					ElementsViewModel.Update();
				}
				ResetHistory();
			}
		}

		public PlanDesignerViewModel PlanDesignerViewModel { get; set; }

		DesignerCanvas _designerCanvas;
		public DesignerCanvas DesignerCanvas
		{
			get { return _designerCanvas; }
			set
			{
				_designerCanvas = value;
				OnPropertyChanged("DesignerCanvas");
			}
		}

		bool CanAddEditRemove()
		{
			return SelectedPlan != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var planDetailsViewModel = new PlanDetailsViewModel();
			if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
			{
				var plan = planDetailsViewModel.Plan;
				var planViewModel = new PlanViewModel(plan, Plans);
				Plans.Add(planViewModel);
				SelectedPlan = planViewModel;

				FiresecManager.PlansConfiguration.Plans.Add(plan);
				FiresecManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}

		public RelayCommand AddSubPlanCommand { get; private set; }
		void OnAddSubPlan()
		{
			var planDetailsViewModel = new PlanDetailsViewModel();
			if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
			{
				var plan = planDetailsViewModel.Plan;
				var planViewModel = new PlanViewModel(plan, Plans);

				SelectedPlan.Children.Add(planViewModel);
				SelectedPlan.Plan.Children.Add(plan);
				planViewModel.Parent = SelectedPlan;
				plan.Parent = SelectedPlan.Plan;

				SelectedPlan.Update();
				SelectedPlan = planViewModel;
				FiresecManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var parent = SelectedPlan.Parent;
			var plan = SelectedPlan.Plan;

			if (parent == null)
			{
				SelectedPlan.IsExpanded = false;
				Plans.Remove(SelectedPlan);
				FiresecManager.PlansConfiguration.Plans.Remove(plan);
			}
			else
			{
				parent.IsExpanded = false;
				parent.Plan.Children.Remove(plan);
				parent.Children.Remove(SelectedPlan);
				parent.Update();
				parent.IsExpanded = true;
			}

			FiresecManager.PlansConfiguration.Update();
			ServiceFactory.SaveService.PlansChanged = true;

			if (Plans.Count > 0)
				SelectedPlan = Plans[0];
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var planDetailsViewModel = new PlanDetailsViewModel(SelectedPlan.Plan);
			if (ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel))
			{
				SelectedPlan.Update();
				DesignerCanvas.Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}

		void OnShowElement(Guid elementUID)
		{
			DesignerCanvas.DeselectAll();

			foreach (var designerItem in DesignerCanvas.Items)
			{
				if (designerItem.ElementBase.UID == elementUID)
				{
					designerItem.IsSelected = true;
				}
			}
		}

		void OnShowElementDevice(Guid deviceUID)
		{
			foreach (var plan in Plans)
			{
				foreach (var elementDevice in plan.Plan.ElementDevices)
				{
					if (elementDevice.UID == deviceUID)
					{
						SelectedPlan = plan;
						OnShowElement(deviceUID);
						return;
					}
				}
			}
		}

		public override void OnShow()
		{
			ServiceFactory.Layout.ShowMenu(new PlansMenuViewModel(this));
			FiresecManager.UpdatePlansConfiguration();
			DevicesViewModel.Update();
			DesignerCanvas.DeselectAll();

			if (ToolboxView.Current != null)
				ToolboxView.Current.AcceptKeyboard = true;
		}

		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);

			if (ToolboxView.Current != null)
				ToolboxView.Current.AcceptKeyboard = false;
		}
	}
}