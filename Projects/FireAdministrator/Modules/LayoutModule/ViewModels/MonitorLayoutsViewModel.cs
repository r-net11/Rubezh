using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace LayoutModule.ViewModels
{
	public class MonitorLayoutsViewModel : MenuViewPartViewModel, ISelectable<Guid>, IInitializable
	{
		public MonitorLayoutViewModel MonitorLayoutViewModel { get; private set; }
		public MonitorLayoutsTreeViewModel MonitorLayoutsTreeViewModel { get; private set; }

		public MonitorLayoutsViewModel()
		{
			MonitorLayoutViewModel = new MonitorLayoutViewModel();
			MonitorLayoutsTreeViewModel = new MonitorLayoutsTreeViewModel(this);
			CreateCommands();
			SetRibbonItems();
		}

		#region ISelectable<Guid> Members

		public void Select(Guid item)
		{
		}

		#endregion

		#region IInitializable Members

		public void Initialize()
		{
			using (new TimeCounter("MonitorLayoutsViewModel.Initialize: {0}"))
			{
				var root = new LayoutViewModel(FiresecManager.LayoutsConfiguration.Root);
				Layouts = new ObservableCollection<LayoutViewModel>(root.Children);
				SelectedLayout = null;
			}
		}

		#endregion

		private ObservableCollection<LayoutViewModel> _layouts;
		public ObservableCollection<LayoutViewModel> Layouts
		{
			get { return _layouts; }
			set
			{
				_layouts = value;
				OnPropertyChanged(() => Layouts);
			}
		}

		private LayoutViewModel _selectedLayout;
		public LayoutViewModel SelectedLayout
		{
			get { return _selectedLayout; }
			set
			{
				using (new TimeCounter("MonitorLayoutsViewModel.SelectedPlan: {0}", true, true))
					if (value != SelectedLayout)
					{
						_selectedLayout = value;
						OnPropertyChanged(() => SelectedLayout);
						MonitorLayoutViewModel.LayoutViewModel = SelectedLayout;
					}
			}
		}

		private object _layoutBuffer;
		private void CreateCommands()
		{
			AddCommand = new RelayCommand(OnAdd);
			AddFolderCommand = new RelayCommand(OnAddFolder);
			AddSubLayoutCommand = new RelayCommand(OnAddSubLayout, CanAddSub);
			AddSubFolderCommand = new RelayCommand(OnAddSubFolder, CanAddSub);
			RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);

			LayoutCopyCommand = new RelayCommand(OnLayoutCopy, CanLayoutCopyCut);
			LayoutCutCommand = new RelayCommand(OnLayoutCut, CanLayoutCopyCut);
			LayoutPasteCommand = new RelayCommand<bool>(OnLayoutPaste, CanLayoutPaste);
			_layoutBuffer = null;
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var layoutDetailsViewModel = new LayoutPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(layoutDetailsViewModel))
				OnLayoutPaste(layoutDetailsViewModel.Layout, true);
		}
		public RelayCommand AddFolderCommand { get; private set; }
		private void OnAddFolder()
		{
			var viewModel = new LayoutFolderPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(viewModel))
				OnLayoutPaste(viewModel.Folder, true);
		}
		public RelayCommand AddSubLayoutCommand { get; private set; }
		private void OnAddSubLayout()
		{
			var layoutDetailsViewModel = new LayoutPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(layoutDetailsViewModel))
				OnLayoutPaste(layoutDetailsViewModel.Layout, false);
		}
		public RelayCommand AddSubFolderCommand { get; private set; }
		private void OnAddSubFolder()
		{
			var viewModel = new LayoutFolderPropertiesViewModel(null);
			if (DialogService.ShowModalWindow(viewModel))
				OnLayoutPaste(viewModel.Folder, false);
		}
		private bool CanAddSub()
		{
			return SelectedLayout != null && SelectedLayout.IsFolder;
		}
		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			string message = string.Format(SelectedLayout.IsFolder != null ? "Вы уверены, что хотите удалить папку '{0}'?" : "Вы уверены, что хотите удалить шаблон '{0}'?", SelectedLayout.Caption);
			if (MessageBoxService.ShowConfirmation(message) == System.Windows.MessageBoxResult.Yes)
				OnLayoutRemove(false);
		}
		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			SaveCancelDialogViewModel dialog = SelectedLayout.IsFolder ? (SaveCancelDialogViewModel)new LayoutFolderPropertiesViewModel(SelectedLayout.LayoutFolder) : new LayoutPropertiesViewModel(SelectedLayout.Layout);
			if (DialogService.ShowModalWindow(dialog))
			{
				SelectedLayout.Update();
				MonitorLayoutViewModel.Update();
				ServiceFactory.SaveService.LayoutsChanged = true;
			}
		}
		private bool CanEditRemove()
		{
			return SelectedLayout != null;
		}
		public RelayCommand LayoutCopyCommand { get; private set; }
		private void OnLayoutCopy()
		{
			using (new WaitWrapper())
				_layoutBuffer = Utils.Clone(SelectedLayout.LayoutObject);
		}
		public RelayCommand LayoutCutCommand { get; private set; }
		private void OnLayoutCut()
		{
			using (new WaitWrapper())
			{
				_layoutBuffer = SelectedLayout.LayoutObject;
				OnLayoutRemove(true);
			}
		}
		private bool CanLayoutCopyCut()
		{
			return SelectedLayout != null;
		}
		public RelayCommand<bool> LayoutPasteCommand { get; private set; }
		private void OnLayoutPaste(bool isRoot)
		{
			if (!isRoot && SelectedLayout == null)
				isRoot = true;
			if (_layoutBuffer is Layout)
			{
				var layout = Utils.Clone((Layout)_layoutBuffer);
				RenewLayout(layout);
				OnLayoutPaste(layout, isRoot);
			}
			else
			{
				var folder = Utils.Clone((LayoutFolder)_layoutBuffer);
				OnLayoutPaste(folder, isRoot);
			}
		}
		private bool CanLayoutPaste(bool isRoot)
		{
			return _layoutBuffer != null;
		}

		private void OnLayoutPaste(Layout layout, bool isRoot)
		{
			using (new WaitWrapper())
			{
				var viewModel = new LayoutViewModel(layout);
				if (isRoot)
					FiresecManager.LayoutsConfiguration.Root.Layouts.Add(layout);
				else
					SelectedLayout.LayoutFolder.Layouts.Add(layout);
				OnLayoutPaste(viewModel, isRoot);
			}
		}
		private void OnLayoutPaste(LayoutFolder folder, bool isRoot)
		{
			using (new WaitWrapper())
			{
				var viewModel = new LayoutViewModel(folder);
				if (isRoot)
					FiresecManager.LayoutsConfiguration.Root.Folders.Add(folder);
				else
					SelectedLayout.LayoutFolder.Folders.Add(folder);
				OnLayoutPaste(viewModel, isRoot);
			}
		}
		private void OnLayoutPaste(LayoutViewModel layoutViewModel, bool isRoot)
		{
			if (isRoot)
				Layouts.Add(layoutViewModel);
			else
			{
				SelectedLayout.AddChild(layoutViewModel);
				SelectedLayout.Update();
				SelectedLayout.IsExpanded = true;
			}
			SelectedLayout = layoutViewModel;
			FiresecManager.LayoutsConfiguration.Update();
			ServiceFactory.SaveService.LayoutsChanged = true;
		}
		private void OnLayoutRemove(bool withChild)
		{
			using (new WaitWrapper())
			{
				var selectedLayout = SelectedLayout;
				var parent = SelectedLayout.Parent;
				var layoutObject = SelectedLayout.LayoutObject;
				if (parent == null)
				{
					Layouts.Remove(selectedLayout);
					if (selectedLayout.IsLayout)
						FiresecManager.LayoutsConfiguration.Root.Layouts.Remove(selectedLayout.Layout);
					else
						FiresecManager.LayoutsConfiguration.Root.Folders.Remove(selectedLayout.LayoutFolder);
					if (!withChild && selectedLayout.IsFolder)
						foreach (var childPlanViewModel in selectedLayout.Children.ToArray())
						{
							Layouts.Add(childPlanViewModel);
							if (childPlanViewModel.IsFolder)
								FiresecManager.LayoutsConfiguration.Root.Folders.Remove(childPlanViewModel.LayoutFolder);
							else
								FiresecManager.LayoutsConfiguration.Root.Layouts.Remove(childPlanViewModel.Layout);
						}
				}
				else
				{
					parent.RemoveChild(selectedLayout);
					if (selectedLayout.IsLayout)
						parent.LayoutFolder.Layouts.Remove(selectedLayout.Layout);
					else
						parent.LayoutFolder.Folders.Remove(selectedLayout.LayoutFolder);
					if (!withChild && selectedLayout.IsFolder)
						foreach (var childPlanViewModel in selectedLayout.Children.ToArray())
						{
							parent.AddChild(childPlanViewModel);
							if (childPlanViewModel.IsFolder)
								parent.LayoutFolder.Folders.Remove(childPlanViewModel.LayoutFolder);
							else
								parent.LayoutFolder.Layouts.Remove(childPlanViewModel.Layout);
						}
					parent.Update();
					parent.IsExpanded = true;
				}
				FiresecManager.LayoutsConfiguration.Update();
				ServiceFactory.SaveService.LayoutsChanged = true;
				SelectedLayout = parent == null ? Layouts.FirstOrDefault() : parent;
			}
		}
		private void RenewLayout(Layout layout)
		{
			layout.UID = Guid.NewGuid();
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
		}
		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить шаблон", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Добавить папку", AddFolderCommand, "/Controls;component/Images/BFolderOpen.png"),
					new RibbonMenuItemViewModel("Добавить дочерний шаблон", AddSubLayoutCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Добавить дочернюю папку", AddSubFolderCommand, "/Controls;component/Images/BFolderOpen.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png") {IsNewGroup = true},
					new RibbonMenuItemViewModel("Удалить", RemoveCommand, "/Controls;component/Images/BDelete.png"),
					new RibbonMenuItemViewModel("Копировать", LayoutCopyCommand, "/Controls;component/Images/BCopy.png") {IsNewGroup=true},
					new RibbonMenuItemViewModel("Вырезать", LayoutCutCommand, "/Controls;component/Images/BCut.png"),
					new RibbonMenuItemViewModel("Вставить", LayoutPasteCommand, true, "/Controls;component/Images/BPaste.png"),
				}, "/Controls;component/Images/BLayouts.png") { Order = 2 }
			};
		}

		public override void OnHide()
		{
			LayoutDesignerViewModel.Instance.SaveLayout();
			base.OnHide();
		}
	}
}
