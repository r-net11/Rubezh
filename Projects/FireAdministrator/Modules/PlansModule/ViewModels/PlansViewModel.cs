using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using PlansModule.Designer;
using PlansModule.Designer.DesignerItems;
using StrazhAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel : ViewPartViewModel
	{
		private double _splitterDistance;
		private readonly GridLength _emptyGridColumn;
		private GridLength _width1;
		private GridLength _width2;
		private GridLength _width3;
		private GridLength _layersHeight;
		private ObservableCollection<PlanViewModel> _plans;
		private PlanViewModel _selectedPlan;

		public PlansViewModel()
		{
			ServiceFactoryBase.Events.GetEvent<ShowPlanElementEvent>().Subscribe(OnShowElement);
			ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Subscribe(OnShowElementDevice);
			ServiceFactoryBase.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);

			AddCommand = new RelayCommand(OnAdd);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, () => SelectedPlan != null);
			RemoveCommand = new RelayCommand(OnRemove, () => SelectedPlan != null);
			EditCommand = new RelayCommand(OnEdit, () => SelectedPlan != null);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, () => SelectedPlan != null);
			AddFolderCommand = new RelayCommand(OnAddFolder);
			AddSubFolderCommand = new RelayCommand(OnAddSubFolder, () => SelectedPlan != null);

			LayerGroupService.Instance.RegisterGroup(Helper.SubPlanAlias, "Ссылки на планы");
			ServiceFactoryBase.Events.GetEvent<DesignerItemFactoryEvent>().Subscribe(e =>
			{
				if (e.Element is ElementSubPlan)
				{
					e.DesignerItem = new DesignerItemSubPlan(e.Element) {IconSource = "/Controls;component/Images/CMap.png"};
				}
			});

			PlanDesignerViewModel = new PlanDesignerViewModel(this);
			PlanDesignerViewModel.IsCollapsedChanged += PlanDesignerViewModel_IsCollapsedChanged;
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

		public GridLength Width1
		{
			get { return _width1; }
			set
			{
				_width1 = value;
				OnPropertyChanged(() => Width1);
			}
		}

		public GridLength Width2
		{
			get { return _width2; }
			set
			{
				_width2 = value;
				OnPropertyChanged(() => Width2);
			}
		}

		public GridLength Width3
		{
			get { return _width3; }
			set
			{
				_width3 = value;
				OnPropertyChanged(() => Width3);
			}
		}

		public GridLength LayersHeight
		{
			get { return _layersHeight; }
			set
			{
				_layersHeight = value;
				OnPropertyChanged(() => LayersHeight);
			}
		}

		public BaseViewModel Menu { get; protected set; }

		public ElementsViewModel ElementsViewModel { get; private set; }

		public PlansTreeViewModel PlansTreeViewModel { get; private set; }

		public ObservableCollection<PlanViewModel> Plans
		{
			get { return _plans; }
			set
			{
				_plans = value;
				OnPropertyChanged(() => Plans);
			}
		}

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

		public Infrastructure.Designer.DesignerCanvas DesignerCanvas
		{
			get { return PlanDesignerViewModel.DesignerCanvas; }
		}

		public PlanDesignerViewModel PlanDesignerViewModel { get; private set; }

		public void Initialize()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.BackgroundImageSource.HasValue && !ServiceFactoryBase.ContentService.CheckIfExists(plan.BackgroundImageSource.Value.ToString()))
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

		private void OnAdd()
		{
			var planDetailsViewModel = new DesignerPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(planDetailsViewModel))
				OnPlanPaste(planDetailsViewModel.Plan, true);
		}

		private void OnAddSubPlan()
		{
			var planDetailsViewModel = new DesignerPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(planDetailsViewModel))
				OnPlanPaste(planDetailsViewModel.Plan, false);
		}

		private void OnRemove()
		{
			var message = string.Format(SelectedPlan.PlanFolder != null ? "Вы действительно хотите удалить папку\n\"{0}\"?" : "Вы действительно хотите удалить графический план \"{0}\"?", SelectedPlan.Caption);
			if (MessageBoxService.ShowConfirmation(message))
				OnPlanRemove(false);
		}

		private void OnEdit()
		{
			var dialog = SelectedPlan.PlanFolder != null
				? (SaveCancelDialogViewModel)new FolderPropertiesViewModel(SelectedPlan.PlanFolder)
				: new DesignerPropertiesViewModel(SelectedPlan.Plan);

			if (DialogService.ShowModalWindow(dialog))
			{
				SelectedPlan.Update();
				DesignerCanvas.Update();
				PlanDesignerViewModel.Update();
				DesignerCanvas.Refresh();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}

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
				if (!DesignerCanvas.SelectedItems.Any())
				{
					var plans = new List<PlanViewModel>();
					GetAllPlans(plans, Plans);
					foreach (var plan in plans.Where(plan => plan.Plan.ElementUnion.Any(elementDevice => deviceUIDs.Contains(elementDevice.UID))))
					{
						SelectedPlan = plan;
						OnShowDevices(deviceUIDs);
						return;
					}
				}
			}
		}

		private void OnShowDevices(ICollection<Guid> deviceUIDs)
		{
			foreach (var item in DesignerCanvas.Items)
				if (deviceUIDs.Contains(item.Element.UID) && item.IsEnabled)
					item.IsSelected = true;
		}

		private static void GetAllPlans(ICollection<PlanViewModel> allPlans, IEnumerable<PlanViewModel> plans)
		{
			if (plans == null) return;

			foreach (var planViewModel in plans)
			{
				allPlans.Add(planViewModel);
				GetAllPlans(allPlans, planViewModel.Children);
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			FiresecManager.UpdatePlansConfiguration();
			DesignerCanvas.DeselectAll();
			ExtensionAttached();

			if (DesignerCanvas.Toolbox != null)
				DesignerCanvas.Toolbox.AcceptKeyboard = true;
			PlanDesignerViewModel.Update();

			if (SelectedPlan == null)
				SelectedPlan = Plans.FirstOrDefault();
		}

		public override void OnHide()
		{
			base.OnHide();
			if (DesignerCanvas.Toolbox != null)
				DesignerCanvas.Toolbox.AcceptKeyboard = false;
		}

		private static void ClearReferences(Plan plan)
		{
			foreach (var p in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var subPlan in p.ElementSubPlans)
					if (subPlan.PlanUID == plan.UID)
						Helper.SetSubPlan(subPlan);
		}

		private void PlanDesignerViewModel_IsCollapsedChanged(object sender, EventArgs e)
		{
			if (Width3 != _emptyGridColumn)
				_splitterDistance = Width3.Value;
			Width1 = new GridLength(1, GridUnitType.Star);
			Width2 = PlanDesignerViewModel.IsCollapsed ? _emptyGridColumn : GridLength.Auto;
			Width3 = PlanDesignerViewModel.IsCollapsed ? _emptyGridColumn : new GridLength(_splitterDistance, GridUnitType.Pixel);
		}

		public RelayCommand AddCommand { get; private set; }
		public RelayCommand AddSubPlanCommand { get; private set; }
		public RelayCommand RemoveCommand { get; private set; }
		public RelayCommand EditCommand { get; private set; }
		public RelayCommand AddFolderCommand { get; private set; }
		public RelayCommand AddSubFolderCommand { get; private set; }
	}
}