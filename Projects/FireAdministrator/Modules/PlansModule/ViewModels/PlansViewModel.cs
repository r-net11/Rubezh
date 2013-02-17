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
using Infrustructure.Plans.Painters;
using Infrastructure.Client.Plans;
using Infrastructure.Common.Windows.ViewModels;

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
			AddFolderCommand = new RelayCommand(OnAddFolder);
			AddSubFolderCommand = new RelayCommand(OnAddSubFolder, CanAddEditRemove);

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
				using (new TimeCounter("\tPlansViewModel.CacheBrushes: {0}"))
					foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
					{
						Helper.UpgradeBackground(plan);
						foreach (var elementBase in PlanEnumerator.Enumerate(plan))
							Helper.UpgradeBackground(elementBase);
					}
				Plans = new ObservableCollection<PlanViewModel>();
				foreach (var plan in FiresecManager.PlansConfiguration.Plans)
					AddPlan(plan, null);

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
				AddPlan(childPlan, planViewModel);
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
				using (new TimeCounter("PlansViewModel.SelectedPlan: {0}", true, true))
				{
					_selectedPlan = value;
					OnPropertyChanged("SelectedPlan");
					DesignerCanvas.Toolbox.IsEnabled = SelectedPlan != null && SelectedPlan.PlanFolder == null;
					PlanDesignerViewModel.Save();
					PlanDesignerViewModel.Initialize(value == null || value.PlanFolder != null ? null : value.Plan);
					using (new TimeCounter("\tPlansViewModel.UpdateElements: {0}"))
						if (value != null)
							ElementsViewModel.Update();
					ResetHistory();
					DesignerCanvas.Toolbox.SetDefault();
					DesignerCanvas.DeselectAll();
				}
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
				SelectedPlan = planViewModel;

				FiresecManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			string message = string.Format(SelectedPlan.PlanFolder != null ? "Вы уверены, что хотите удалить папку '{0}'?" : "Вы уверены, что хотите удалить план '{0}'?", SelectedPlan.Caption);
			if (MessageBoxService.ShowConfirmation(message) == System.Windows.MessageBoxResult.Yes)
			{
				var selectedPlan = SelectedPlan;
				var parent = selectedPlan.Parent;
				var plan = SelectedPlan.Plan;
				ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Publish(DesignerCanvas.Items.Select(item => item.Element).ToList());
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
				SelectedPlan = parent == null ? Plans.FirstOrDefault() : parent;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			SaveCancelDialogViewModel dialog = SelectedPlan.PlanFolder != null ? (SaveCancelDialogViewModel)new FolderPropertiesViewModel(SelectedPlan.PlanFolder) : new DesignerPropertiesViewModel(SelectedPlan.Plan);
			if (DialogService.ShowModalWindow(dialog))
			{
				SelectedPlan.Update();
				DesignerCanvas.Update();
				PlanDesignerViewModel.Update();
				DesignerCanvas.Refresh();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		public RelayCommand AddFolderCommand { get; private set; }
		private void OnAddFolder()
		{
			var viewModel = new FolderPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(viewModel))
			{
				var planFolder = viewModel.PlanFolder;
				var planViewModel = new PlanViewModel(planFolder);
				Plans.Add(planViewModel);
				SelectedPlan = planViewModel;
				FiresecManager.PlansConfiguration.Plans.Add(planFolder);
				FiresecManager.PlansConfiguration.Update();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
		public RelayCommand AddSubFolderCommand { get; private set; }
		private void OnAddSubFolder()
		{
			var viewModel = new FolderPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(viewModel))
			{
				var planFolder = viewModel.PlanFolder;
				var planViewModel = new PlanViewModel(planFolder);
				SelectedPlan.Children.Add(planViewModel);
				SelectedPlan.Plan.Children.Add(planFolder);
				SelectedPlan.Update();
				SelectedPlan = planViewModel;
				FiresecManager.PlansConfiguration.Update();
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
				ExtensionAttached();

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