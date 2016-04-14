using Common;
using Controls.Menu.ViewModels;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using PlansModule.Designer;
using PlansModule.Designer.DesignerItems;
using RubezhAPI.Models;
using RubezhClient;
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
		public ObservableCollection<MenuButtonViewModel> PlansMenu { get; private set; }

		public ElementsViewModel ElementsViewModel { get; private set; }
		public PlansTreeViewModel PlansTreeViewModel { get; private set; }

		public PlansViewModel()
		{
			ServiceFactory.Events.GetEvent<ShowPlanElementEvent>().Subscribe(OnShowElement);
			ServiceFactory.Events.GetEvent<FindElementEvent>().Subscribe(OnShowElementDevice);
			ServiceFactory.Events.GetEvent<RemoveGKDeviceEvent>().Subscribe(OnRemoveElementDevice);
			ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);

			AddCommand = new RelayCommand(OnAdd);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);
			RemoveCommand = new RelayCommand(OnRemove, CanAddEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanAddEditRemove);
			MoveDownCommand = new RelayCommand(this.OnMoveDown, this.CanMoveDown);
			MoveUpCommand = new RelayCommand(this.OnMoveUp, this.CanMoveUp);
			AddSubPlanCommand = new RelayCommand(OnAddSubPlan, CanAddEditRemove);
			AddFolderCommand = new RelayCommand(OnAddFolder);
			AddSubFolderCommand = new RelayCommand(OnAddSubFolder, CanAddEditRemove);

			LayerGroupService.Instance.RegisterGroup(Helper.SubPlanAlias, "Ссылки на планы");
			ServiceFactory.Events.GetEvent<DesignerItemFactoryEvent>().Subscribe((e) =>
			{
				if (e.Element is IElementSubPlan)
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
			this.InitializeMenus();

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
			foreach (var plan in ClientManager.PlansConfiguration.AllPlans.Where(x => !x.IsAsynchronousLoad))
			{
				if (plan.BackgroundImageSource.HasValue && !ServiceFactory.ContentService.CheckIfExists(plan.BackgroundImageSource.Value.ToString()))
					plan.BackgroundImageSource = null;
				Helper.UpgradeBackground(plan);
				foreach (var elementBase in PlanEnumerator.Enumerate(plan))
					Helper.UpgradeBackground(elementBase);
			}

			SelectedPlan = null;
			Plans = new ObservableCollection<PlanViewModel>();
			foreach (var plan in ClientManager.PlansConfiguration.Plans)
				AddPlan(plan, null);
			if (SelectedPlan != null)
				SelectedPlan.ExpandToThis();
		}

		/// <summary>
		/// Initializes Menu Bars.
		/// </summary>
		private void InitializeMenus()
		{
			this.Menu = new PlansMenuViewModel(this);
			this.PlansMenu = new ObservableCollection<MenuButtonViewModel>()
			{
				new MenuButtonViewModel(this.AddFolderCommand, "FolderOpen", "Добавить папку"),
				new MenuButtonViewModel(this.AddCommand, "Add", "Добавить план"),
				new MenuButtonViewModel(this.EditCommand, "Edit", "Редактировать план"),
				new MenuButtonViewModel(this.RemoveCommand, "Delete", "Удалить план"),
				new MenuButtonViewModel(this.MoveDownCommand, "arrowDown", "Переместить вниз"),
				new MenuButtonViewModel(this.MoveUpCommand, "arrowUp", "Переместить вверх"),
			};
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
			string message = string.Format(SelectedPlan.PlanFolder != null ? "Вы уверены, что хотите удалить папку '{0}'?" : "Вы уверены, что хотите удалить план '{0}'?", SelectedPlan.Caption);
			if (MessageBoxService.ShowConfirmation(message))
			{
				OnPlanRemove(false);
			}

		}
		public RelayCommand MoveDownCommand { get; private set; }
		private void OnMoveDown()
		{
			PlanViewModel selectedPlan = this.SelectedPlan;
			if (selectedPlan.Parent == null)
			{
				MoveItem(this.Plans, selectedPlan, +1);
				MoveItem(ClientManager.PlansConfiguration.Plans, selectedPlan.Plan, +1);
			}
			else
			{
				MoveItem(this.SelectedPlan.Parent.Nodes, selectedPlan, +1);
				MoveItem(ClientManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == selectedPlan.Parent.Plan.UID).Children, selectedPlan.Plan, +1);
			}
			this.SelectedPlan = selectedPlan;
			ServiceFactory.SaveService.PlansChanged = true;
		}
		private bool CanMoveDown()
		{
			if (this.SelectedPlan == null)
				return false;
			int index = 0;
			int count = 0;
			if (this.SelectedPlan.Parent == null)
			{
				index = this.Plans.IndexOf(this.SelectedPlan);
				count = this.Plans.Count;
			}
			else
			{
				index = this.SelectedPlan.Parent.Nodes.IndexOf(this.SelectedPlan);
				count = this.SelectedPlan.Parent.Nodes.Count;
			}
			return index < count - 1;
		}

		public RelayCommand MoveUpCommand { get; private set; }
		private void OnMoveUp()
		{
			PlanViewModel selectedPlan = this.SelectedPlan;
			if (selectedPlan.Parent == null)
			{
				MoveItem(this.Plans, selectedPlan, -1);
				MoveItem(ClientManager.PlansConfiguration.Plans, selectedPlan.Plan, -1);
			}
			else
			{
				MoveItem(this.SelectedPlan.Parent.Nodes, selectedPlan, -1);
				MoveItem(ClientManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == selectedPlan.Parent.Plan.UID).Children, selectedPlan.Plan, -1);
			}
			this.SelectedPlan = selectedPlan;
			ServiceFactory.SaveService.PlansChanged = true;
		}
		private bool CanMoveUp()
		{
			if (this.SelectedPlan == null)
				return false;
			if (this.SelectedPlan.Parent == null)
				return this.Plans.IndexOf(this.SelectedPlan) > 0;
			else
				return this.SelectedPlan.Parent.Nodes.IndexOf(this.SelectedPlan) > 0;
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
				ClientManager.PlansConfiguration.Plans.Add(planFolder);
				ClientManager.PlansConfiguration.Update();
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
				ClientManager.PlansConfiguration.Update();
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
		private void OnRemoveElementDevice(Guid deviceUID)
		{
			IEnumerable<ElementGKDevice> allDevices = this.Plans
				.SelectMany(plan => this.GetAllChildren(plan))
				.SelectMany(plan => plan.Plan.ElementGKDevices)
				.Where(device => device.DeviceUID == deviceUID)
				.ToArray();
			foreach (var device in allDevices)
			{
				DesignerCanvas.RemoveDesignerItem(device);
				ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Publish(new List<ElementBase>() { device });
			}
			ServiceFactory.SaveService.PlansChanged = true;
		}

		/// <summary>
		/// Retrieves a Plan an all its Child Plans.
		/// </summary>
		/// <param name="plan">Plan to get Children for.</param>
		/// <returns>Collection of Plans.</returns>
		private IEnumerable<PlanViewModel> GetAllChildren(PlanViewModel plan)
		{
			return new PlanViewModel[] { plan }
				.Union(plan.Children.SelectMany(child => this.GetAllChildren(child)));
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
			using (new WaitWrapper())
			using (new TimeCounter("PlansViewModel.OnShow: {0}"))
			{
				base.OnShow();
				using (new TimeCounter("PlansViewModel.UpdatePlansConfiguration: {0}"))
					ClientManager.UpdatePlansConfiguration();
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
			foreach (var p in ClientManager.PlansConfiguration.AllPlans)
			{
				foreach (var subPlan in p.ElementSubPlans)
					if (subPlan.PlanUID == plan.UID)
						Helper.SetSubPlan(subPlan);
				foreach (var subPlan in p.ElementPolygonSubPlans)
					if (subPlan.PlanUID == plan.UID)
						Helper.SetSubPlan(subPlan);
			}
		}

		private static void MoveItem<T>(IList<T> parent, T item, int increment)
		{
			int itemIndex = parent.IndexOf(item);
			T temp = item;
			parent.RemoveAt(itemIndex);
			parent.Insert(itemIndex + increment, temp);
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