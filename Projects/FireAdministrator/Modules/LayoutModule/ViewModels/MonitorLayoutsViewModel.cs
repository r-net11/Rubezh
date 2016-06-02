using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Common;
using Localization.Layout.Common;
using Localization.Layout.ViewModels;
using StrazhAPI.Models.Layouts;
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
		public static MonitorLayoutsViewModel Instance { get; private set; }
		public MonitorLayoutViewModel MonitorLayoutViewModel { get; private set; }
		public MonitorLayoutsTreeViewModel MonitorLayoutsTreeViewModel { get; private set; }
		public LayoutUsersViewModel LayoutUsersViewModel { get; private set; }

		public MonitorLayoutsViewModel()
		{
			MonitorLayoutViewModel = new MonitorLayoutViewModel();
			MonitorLayoutsTreeViewModel = new MonitorLayoutsTreeViewModel(this);
			CreateCommands();
			SetRibbonItems();
			Menu = new MonitorLayoutsMenuViewModel(this);
			Instance = this;
		}

		#region ISelectable<Guid> Members

		public void Select(Guid guid)
		{
			var layout = Layouts.FirstOrDefault(item => item.Layout.UID == guid);
			if (layout != null)
				SelectedLayout = layout;
		}

		#endregion

		#region IInitializable Members

		public void Initialize()
		{
			using (new TimeCounter("MonitorLayoutsViewModel.Initialize: {0}"))
			{
				LayoutUsersViewModel = new LayoutUsersViewModel();
				Layouts = new ObservableCollection<LayoutViewModel>();
				ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(Layouts);
				view.CustomSort = new LayoutViewModelComparer();
				foreach (var layout in FiresecManager.LayoutsConfiguration.Layouts)
					Layouts.Add(new LayoutViewModel(layout));
				view.MoveCurrentToFirst();
				SelectedLayout = (LayoutViewModel)view.CurrentItem;
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

		private Layout _layoutBuffer;
		private void CreateCommands()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);

			LayoutCopyCommand = new RelayCommand(OnLayoutCopy, CanLayoutCopyCut);
			LayoutCutCommand = new RelayCommand(OnLayoutCut, CanLayoutCopyCut);
			LayoutPasteCommand = new RelayCommand(OnLayoutPaste, CanLayoutPaste);
			_layoutBuffer = null;
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			var layout = new Layout();
			var adminUser = FiresecManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == "adm");
			if (adminUser != null)
			{
				layout.Users.Add(adminUser.UID);
			}
			var layoutDetailsViewModel = new LayoutPropertiesViewModel(layout, LayoutUsersViewModel);
			if (DialogService.ShowModalWindow(layoutDetailsViewModel))
				OnLayoutPaste(layoutDetailsViewModel.Layout);
		}
		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
            if (MessageBoxService.ShowConfirmation(string.Format(CommonViewModels.MonitorLayouts_OnRemove, SelectedLayout.Caption)))
				OnLayoutRemove();
		}
		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			if (DialogService.ShowModalWindow(new LayoutPropertiesViewModel(SelectedLayout.Layout, LayoutUsersViewModel)))
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
			_layoutBuffer = Utils.Clone(SelectedLayout.Layout);
		}
		public RelayCommand LayoutCutCommand { get; private set; }
		private void OnLayoutCut()
		{
			_layoutBuffer = SelectedLayout.Layout;
			OnLayoutRemove();
		}
		private bool CanLayoutCopyCut()
		{
			return SelectedLayout != null;
		}
		public RelayCommand LayoutPasteCommand { get; private set; }
		private void OnLayoutPaste()
		{
			OnLayoutPaste(_layoutBuffer, true);
		}
		private bool CanLayoutPaste()
		{
			return _layoutBuffer != null;
		}
		private void OnLayoutPaste(Layout layout, bool clone = false)
		{
			if (clone)
			{
				layout = Utils.Clone(layout);
				RenewLayout(layout);
			}
			var viewModel = new LayoutViewModel(layout);
			FiresecManager.LayoutsConfiguration.Layouts.Add(layout);
			Layouts.Add(viewModel);
			SelectedLayout = viewModel;
			FiresecManager.LayoutsConfiguration.Update();
			ServiceFactory.SaveService.LayoutsChanged = true;
		}
		private void OnLayoutRemove()
		{
			var collection = (ListCollectionView)CollectionViewSource.GetDefaultView(Layouts);
			var selectedLayout = SelectedLayout;
			var index = collection.IndexOf(SelectedLayout);
			Layouts.Remove(selectedLayout);
			FiresecManager.LayoutsConfiguration.Layouts.Remove(selectedLayout.Layout);
			FiresecManager.LayoutsConfiguration.Update();
			ServiceFactory.SaveService.LayoutsChanged = true;
			if (collection.Count > 0)
				SelectedLayout = (LayoutViewModel)collection.GetItemAt(index >= collection.Count ? collection.Count - 1 : index);
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
				new RibbonMenuItemViewModel(CommonViewModels.MonitorLayouts_Edition, new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel(CommonResources.AddLayout, AddCommand, "BAdd"),
					new RibbonMenuItemViewModel(CommonResources.Edit, EditCommand, "BEdit"),
					new RibbonMenuItemViewModel(CommonResources.Delete, RemoveCommand, "BDelete"),
					new RibbonMenuItemViewModel(CommonResources.Copy, LayoutCopyCommand, "BCopy") {IsNewGroup=true},
					new RibbonMenuItemViewModel(CommonResources.Cut, LayoutCutCommand, "BCut"),
					new RibbonMenuItemViewModel(CommonResources.Paste, LayoutPasteCommand, true, "BPaste"),
				}, "BLayouts") { Order = 2 }
			};
		}

		public override void OnHide()
		{
			LayoutDesignerViewModel.Instance.SaveLayout();
			base.OnHide();
		}
		public void SaveConfiguration()
		{
			if (IsActive)
				LayoutDesignerViewModel.Instance.SaveLayout();
		}

		protected override void KeyPressed(KeyEventArgs e)
		{
			if (!e.Handled)
				LayoutDesignerViewModel.Instance.KeyPressed(e);
			base.KeyPressed(e);
		}
	}
}