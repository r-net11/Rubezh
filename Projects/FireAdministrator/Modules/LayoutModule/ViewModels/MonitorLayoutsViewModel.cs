using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Common;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Ribbon;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace LayoutModule.ViewModels
{
	public class MonitorLayoutsViewModel : MenuViewPartViewModel, ISelectable<Guid>, IInitializable
	{
		static int _copyCounter;
		public static MonitorLayoutsViewModel Instance { get; private set; }
		public MonitorLayoutViewModel MonitorLayoutViewModel { get; private set; }
		public LayoutUsersViewModel LayoutUsersViewModel { get; private set; }

		public MonitorLayoutsViewModel()
		{
			MonitorLayoutViewModel = new MonitorLayoutViewModel();
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
				foreach (var layout in ClientManager.LayoutsConfiguration.Layouts)
					Layouts.Add(new LayoutViewModel(layout));
				SortLayouts();
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
			CloseLayoutPartCommand = new RelayCommand(OnCloseLayoutPart);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);

			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			
			_layoutBuffer = null;
		}

		public RelayCommand CloseLayoutPartCommand { get; set; }

		void OnCloseLayoutPart()
		{
			LayoutDesignerViewModel.Instance.LayoutParts.Remove(LayoutDesignerViewModel.Instance.ActiveLayoutPart);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			LayoutUsersViewModel.Update();
			var otherCaptions = Layouts.Select(x => x.Caption).ToList();
			var layout = new Layout(otherCaptions);
			var adminUser = ClientManager.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == "adm");
			if (adminUser != null)
			{
				layout.Users.Add(adminUser.UID);
			}
			var layoutDetailsViewModel = new LayoutPropertiesViewModel(layout, LayoutUsersViewModel, otherCaptions);
			if (DialogService.ShowModalWindow(layoutDetailsViewModel))
				OnPaste(layoutDetailsViewModel.Layout);
		}
		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			if (MessageBoxService.ShowConfirmation(string.Format("Вы уверены, что хотите удалить макет '{0}'?", SelectedLayout.Caption)))
			{
				using (new WaitWrapper())
				{
					var collection = (ListCollectionView)CollectionViewSource.GetDefaultView(Layouts);
					var selectedLayout = SelectedLayout;
					var index = collection.IndexOf(SelectedLayout);
					Layouts.Remove(selectedLayout);
					ClientManager.LayoutsConfiguration.Layouts.Remove(selectedLayout.Layout);
					ClientManager.LayoutsConfiguration.Update();
					ServiceFactory.SaveService.LayoutsChanged = true;
					if (collection.Count > 0)
						SelectedLayout = (LayoutViewModel)collection.GetItemAt(index >= collection.Count ? collection.Count - 1 : index);
				}
			}
		}
		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			LayoutUsersViewModel.Update();
			var otherCaptions = Layouts.Select(x => x.Caption).Where(x => x != SelectedLayout.Layout.Caption).ToList();
			if (DialogService.ShowModalWindow(new LayoutPropertiesViewModel(SelectedLayout.Layout, LayoutUsersViewModel, otherCaptions)))
			{
				SelectedLayout.Update();
				MonitorLayoutViewModel.Update();
				ServiceFactory.SaveService.LayoutsChanged = true;
			}
		}
		private bool CanEditDelete()
		{
			return SelectedLayout != null;
		}
		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			using (new WaitWrapper())
				_layoutBuffer = Utils.Clone(SelectedLayout.Layout);
			_copyCounter = 0;
		}
		private bool CanCopy()
		{
			return SelectedLayout != null;
		}
		public RelayCommand PasteCommand { get; private set; }
		private void OnPaste()
		{
			OnPaste(_layoutBuffer, true);
		}
		private bool CanPaste()
		{
			return _layoutBuffer != null;
		}
		private void OnPaste(Layout layout, bool clone = false)
		{
			using (new WaitWrapper())
			{
				if (clone)
				{
					layout = Utils.Clone(layout);
					RenewLayout(layout);
				}
				var viewModel = new LayoutViewModel(layout);
				ClientManager.LayoutsConfiguration.Layouts.Add(layout);
				Layouts.Add(viewModel);
				SortLayouts();
				SelectedLayout = viewModel;
				ClientManager.LayoutsConfiguration.Update();
				ServiceFactory.SaveService.LayoutsChanged = true;
			}
		}
		private void RenewLayout(Layout layout)
		{
			layout.UID = Guid.NewGuid();
			if (_copyCounter == 0)
			{
				layout.Caption = string.Format("{0} - копия", layout.Caption);
			}
			else
			{
				layout.Caption = string.Format("{0} - копия({1})", layout.Caption, _copyCounter);
			}
			_copyCounter++;
		}
		void SortLayouts()
		{
			Layouts = new ObservableCollection<LayoutViewModel>(Layouts.OrderBy(x => x.Caption));
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
					new RibbonMenuItemViewModel("Добавить макет", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
					new RibbonMenuItemViewModel("Копировать", CopyCommand, "BCopy") {IsNewGroup=true},
					new RibbonMenuItemViewModel("Вставить", PasteCommand, true, "BPaste"),
				}, "BLayouts") { Order = 2 }
			};
		}

		public override void OnShow()
		{
			if(SelectedLayout != null)
				SelectedLayout.Update();
			MonitorLayoutViewModel.Update();
			base.OnShow();
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
	}
}