using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models.Skud;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using SkudModule.Properties;

namespace SkudModule.ViewModels
{
	public class EmployeeCardIndexViewModel : MenuViewPartViewModel, IEditingViewModel
	{
		public ObservableCollection<EmployeeCardViewModel> EmployeeCardIndex { get; set; }
		private EmployeeCardViewModel _selectedCard;
		private EmployeeCardIndexFilter _filter;

		public EmployeeCardViewModel SelectedEmployeeCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged("SelectedEmployeeCard");
			}
		}

		public EmployeeCardIndexViewModel()
		{
			Menu = new EmployeeCardIndexMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			RefreshCommand = new RelayCommand(OnRefresh);
			FilterCommand = new RelayCommand(OnFilter);
			ClearFilterCommand = new RelayCommand(OnClearFilter, CanClearFilter);
			EmployeeCardIndex = new ObservableCollection<EmployeeCardViewModel>();
			_filter = new EmployeeCardIndexFilter();
			SetRibbonItems();
		}

		public void Initialize()
		{
            //var list = FiresecManager.GetEmployees(_filter);
            EmployeeCardIndex.Clear();
            //if (list != null)
            //    foreach (var employee in list)
            //        EmployeeCardIndex.Add(new EmployeeCardViewModel(employee));
            SelectedEmployeeCard = EmployeeCardIndex.FirstOrDefault();
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			BeginEdit();
		}
		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			BeginEdit(SelectedEmployeeCard);
		}
		bool CanEditRemove()
		{
			return SelectedEmployeeCard != null;
		}
		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowConfirmation(Resources.DeleteEmployeeConfirmation) == MessageBoxResult.Yes)
			{
				if (FiresecManager.DeleteEmployeeCard(SelectedEmployeeCard.EmployeeCard))
				{
					int index = EmployeeCardIndex.IndexOf(SelectedEmployeeCard);
					EmployeeCardIndex.Remove(SelectedEmployeeCard);
					if (EmployeeCardIndex.IsNotNullOrEmpty())
						SelectedEmployeeCard = index < EmployeeCardIndex.Count ? EmployeeCardIndex[index] : EmployeeCardIndex[EmployeeCardIndex.Count - 1];
				}
				else
					MessageBoxService.ShowError(Resources.DeleteEmployeeFailed);
			}
		}
		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
		public RelayCommand FilterCommand { get; private set; }
		void OnFilter()
		{
			EmployeeCardIndexFilterViewModel viewModel = new EmployeeCardIndexFilterViewModel(_filter);
			if (DialogService.ShowModalWindow(viewModel))
				Initialize();
		}
		public RelayCommand ClearFilterCommand { get; private set; }
		void OnClearFilter()
		{
			_filter = new EmployeeCardIndexFilter();
			Initialize();
		}
		bool CanClearFilter()
		{
			return
				!string.IsNullOrEmpty(_filter.ClockNumber) ||
				!string.IsNullOrEmpty(_filter.FirstName) ||
				!string.IsNullOrEmpty(_filter.LastName) ||
				!string.IsNullOrEmpty(_filter.SecondName) ||
				_filter.DepartmentId.HasValue ||
				_filter.GroupId.HasValue ||
				_filter.PositionId.HasValue;
		}

		private void BeginEdit(EmployeeCardViewModel card = null)
		{
			EmployeeCardDetailsViewModel viewModel = new EmployeeCardDetailsViewModel(card);
			if (DialogService.ShowModalWindow(viewModel))
			{
				if (card == null)
				{
					card = viewModel.EmployeeCardViewModel;
					EmployeeCardIndex.Add(card);
				}
				else
					card.Update();
				SelectedEmployeeCard = card;
			}
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
					new RibbonMenuItemViewModel("Обновить", RefreshCommand, "/Controls;component/Images/BRefresh.png"),
					new RibbonMenuItemViewModel("Фильтр", FilterCommand, "/Controls;component/Images/BFilter.png"),
					new RibbonMenuItemViewModel("Очистить фильтр", ClearFilterCommand, "/Controls;component/Images/BFilterClear.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 2 } ,
			};
		}
	}
}