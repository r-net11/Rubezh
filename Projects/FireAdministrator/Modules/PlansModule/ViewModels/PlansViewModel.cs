using Common;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using PlansModule.Designer;
using PlansModule.Designer.DesignerItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel : ViewPartViewModel
	{
		public BaseViewModel Menu { get; protected set; }
		public ElementsViewModel ElementsViewModel { get; private set; }
		public PlansTreeViewModel PlansTreeViewModel { get; private set; }

		public PlansViewModel()
		{
			ServiceFactory.Events.GetEvent<ShowPlanElementEvent>().Subscribe(OnShowElement);
			ServiceFactory.Events.GetEvent<FindElementEvent>().Subscribe(OnShowElementDevice);
			ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);

			AddCommand = new RelayCommand(OnAdd);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);
			RemoveCommand = new RelayCommand(OnRemove, CanAddEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanAddEditRemove);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);
			AddFolderCommand = new RelayCommand(OnAddFolder);
			AddSubFolderCommand = new RelayCommand(OnAddSubFolder, CanAddEditRemove);

			LayerGroupService.Instance.RegisterGroup(Helper.SubPlanAlias, "Ссылки на планы");
			ServiceFactory.Events.GetEvent<DesignerItemFactoryEvent>().Subscribe((e) =>
			{
				if (e.Element is ElementSubPlan)
				{
					e.DesignerItem = new DesignerItemSubPlan(e.Element);
					e.DesignerItem.IconSource = "/Controls;component/Images/CMap.png";
				}
			});

			PlanDesignerViewModel = new PlanDesignerViewModel(this);
			PlanDesignerViewModel.IsCollapsedChanged += new EventHandler(PlanDesignerViewModel_IsCollapsedChanged);
			OnPropertyChanged(() => PlanDesignerViewModel);
			PlanDesignerViewModel.DesignerCanvas.ZoomChanged();
			ElementsViewModel = new ElementsViewModel(PlanDesignerViewModel.DesignerCanvas);

			InitializeCopyPaste();
			PlansTreeViewModel = new PlansTreeViewModel(this);
			CreatePages();
			_planExtensions = new List<Infrustructure.Plans.IPlanExtension<Plan>>();
			Menu = new PlansMenuViewModel(this);
			_splitterDistance = RegistrySettingsHelper.GetDouble("Administrator.Plans.SplitterDistance");
			if (_splitterDistance == 0)
				_splitterDistance = 300;
			_emptyGridColumn = new GridLength(0, GridUnitType.Pixel);
			Width1 = new GridLength(1, GridUnitType.Star);
			Width2 = GridLength.Auto;
			Width3 = new GridLength(_splitterDistance, GridUnitType.Pixel);
			var layerDistance = RegistrySettingsHelper.GetDouble("Administrator.Plans.LayerDistance");
			LayersHeight = new GridLength(layerDistance == 0 ? 500 : layerDistance, GridUnitType.Pixel);
			ApplicationService.ShuttingDown += () =>
			{
				RegistrySettingsHelper.SetDouble("Administrator.Plans.SplitterDistance", Width3 == _emptyGridColumn ? _splitterDistance : Width3.Value);
				RegistrySettingsHelper.SetDouble("Administrator.Plans.LayerDistance", LayersHeight.Value);
			};
		}

		public void Initialize()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.BackgroundImageSource.HasValue && !ServiceFactory.ContentService.CheckIfExists(plan.BackgroundImageSource.Value.ToString()))
					plan.BackgroundImageSource = null;
				Helper.UpgradeBackground(plan);
				foreach (var elementBase in PlanEnumerator.Enumerate(plan))
					Helper.UpgradeBackground(elementBase);
			}
			SelectedPlan = null;
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.Plans)
				AddPlan(plan, null);
			if (SelectedPlan != null)
				SelectedPlan.ExpandToThis();
		}
		private PlanViewModel AddPlan(Plan plan, PlanViewModel parentPlanViewModel)
		{
			var planViewModel = new PlanViewModel(plan);
			if (parentPlanViewModel == null)
				Plans.Add(planViewModel);
			else
				parentPlanViewModel.AddChild(planViewModel);
			if (SelectedPlan == null && !planViewModel.IsFolder)
				SelectedPlan = planViewModel;

			foreach (var childPlan in plan.Children)
				AddPlan(childPlan, planViewModel);
			return planViewModel;
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
				DesignerCanvas.Toolbox.IsEnabled = SelectedPlan != null && SelectedPlan.PlanFolder == null;
				PlanDesignerViewModel.Save();
				PlanDesignerViewModel.Initialize(value == null || value.PlanFolder != null ? null : value.Plan);
				ElementsViewModel.Update();
				DesignerCanvas.Toolbox.SetDefault();
				DesignerCanvas.DeselectAll();
				UpdateTabIndex();
			}
		}

		public PlanDesignerViewModel PlanDesignerViewModel { get; private set; }
		public Infrastructure.Designer.DesignerCanvas DesignerCanvas
		{
			get { return PlanDesignerViewModel.DesignerCanvas; }
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var planDetailsViewModel = new DesignerPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(planDetailsViewModel))
				OnPlanPaste(planDetailsViewModel.Plan, true);
		}
		public RelayCommand AddSubPlanCommand { get; private set; }
		private void OnAddSubPlan()
		{
			var planDetailsViewModel = new DesignerPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(planDetailsViewModel))
				OnPlanPaste(planDetailsViewModel.Plan, false);
		}
		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			string message = string.Format(SelectedPlan.PlanFolder != null ? "Вы действительно хотите удалить папку\n\"{0}\"?" : "Вы действительно хотите удалить графический план \"{0}\"?", SelectedPlan.Caption);
			if (MessageBoxService.ShowConfirmation(message))
				OnPlanRemove(false);
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
				SelectedPlan.AddChild(planViewModel);
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

		private void OnSelectPlan(Guid planUID)
		{
			DesignerCanvas.Toolbox.SetDefault();
			DesignerCanvas.DeselectAll();
			var plans = new List<PlanViewModel>();
			GetAllPlans(plans, Plans);
			var plan = plans.FirstOrDefault(item => item.Plan.UID == planUID);
			if (plan != null)
				SelectedPlan = plan;
		}
		private void OnShowElement(ShowOnPlanArgs<Guid> arg)
		{
			DesignerCanvas.Toolbox.SetDefault();
			DesignerCanvas.DeselectAll();
			if (arg.PlanUID.HasValue)
				OnSelectPlan(arg.PlanUID.Value);
			foreach (var item in DesignerCanvas.Items)
				if (arg.Value == item.Element.UID && item.IsEnabled)
					item.IsSelected = true;
		}
		private void OnShowElementDevice(List<Guid> deviceUIDs)
		{
			DesignerCanvas.Toolbox.SetDefault();
			DesignerCanvas.DeselectAll();
			if (deviceUIDs.Count > 0)
			{
				OnShowDevices(deviceUIDs);
				if (DesignerCanvas.SelectedItems.Count() == 0)
				{
					var plans = new List<PlanViewModel>();
					GetAllPlans(plans, Plans);
					foreach (var plan in plans)
						foreach (var elementDevice in plan.Plan.ElementUnion)
							if (deviceUIDs.Contains(elementDevice.UID))
							{
								SelectedPlan = plan;
								OnShowDevices(deviceUIDs);
								return;
							}
				}
			}
		}
		private void OnShowDevices(List<Guid> deviceUIDs)
		{
			foreach (var item in DesignerCanvas.Items)
				if (deviceUIDs.Contains(item.Element.UID) && item.IsEnabled)
					item.IsSelected = true;
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
			using (new TimeCounter("PlansViewModel.OnShow: {0}"))
			{
				base.OnShow();
				using (new TimeCounter("PlansViewModel.UpdatePlansConfiguration: {0}"))
					FiresecManager.UpdatePlansConfiguration();
				DesignerCanvas.DeselectAll();
				ExtensionAttached();

				if (DesignerCanvas.Toolbox != null)
					DesignerCanvas.Toolbox.AcceptKeyboard = true;
				PlanDesignerViewModel.Update();
			}
			if (SelectedPlan == null)
				SelectedPlan = Plans.FirstOrDefault();
		}
		public override void OnHide()
		{
			base.OnHide();
			if (DesignerCanvas.Toolbox != null)
				DesignerCanvas.Toolbox.AcceptKeyboard = false;
		}

		private void ClearReferences(Plan plan)
		{
			foreach (var p in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var subPlan in p.ElementSubPlans)
					if (subPlan.PlanUID == plan.UID)
						Helper.SetSubPlan(subPlan);
		}

		private double _splitterDistance;
		private GridLength _emptyGridColumn;

		private GridLength _width1;
		public GridLength Width1
		{
			get { return _width1; }
			set
			{
				_width1 = value;
				OnPropertyChanged(() => Width1);
			}
		}
		private GridLength _width2;
		public GridLength Width2
		{
			get { return _width2; }
			set
			{
				_width2 = value;
				OnPropertyChanged(() => Width2);
			}
		}
		private GridLength _width3;
		public GridLength Width3
		{
			get { return _width3; }
			set
			{
				_width3 = value;
				OnPropertyChanged(() => Width3);
			}
		}
		private GridLength _layersHeight;
		public GridLength LayersHeight
		{
			get { return _layersHeight; }
			set
			{
				_layersHeight = value;
				OnPropertyChanged(() => LayersHeight);
			}
		}
		private void PlanDesignerViewModel_IsCollapsedChanged(object sender, EventArgs e)
		{
			if (Width3 != _emptyGridColumn)
				_splitterDistance = Width3.Value;
			Width1 = new GridLength(1, GridUnitType.Star);
			Width2 = PlanDesignerViewModel.IsCollapsed ? _emptyGridColumn : GridLength.Auto;
			Width3 = PlanDesignerViewModel.IsCollapsed ? _emptyGridColumn : new GridLength(_splitterDistance, GridUnitType.Pixel);
		}
	}
}