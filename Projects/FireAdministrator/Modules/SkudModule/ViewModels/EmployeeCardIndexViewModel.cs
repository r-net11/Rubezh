using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using FiresecAPI.Models.Skud;
using Controls.MessageBox;
using System.Windows;
using SkudModule.Properties;

namespace SkudModule.ViewModels
{
	public class EmployeeCardIndexViewModel : RegionViewModel, IEditingViewModel
	{
		public ObservableCollection<EmployeeCardViewModel> EmployeeCardIndex { get; set; }
		private EmployeeCardViewModel _selectedCard;
		private EmployeeCardIndexMenuViewModel _сardIndexMenuViewModel;

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
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			RefreshCommand = new RelayCommand(OnRefresh);
			FilterCommand = new RelayCommand(OnFilter);
			ClearFilterCommand = new RelayCommand(OnClearFilter);
			EmployeeCardIndex = new ObservableCollection<EmployeeCardViewModel>();
			_сardIndexMenuViewModel = new EmployeeCardIndexMenuViewModel(this);
		}

		public void Initialize()
		{
			var list = FiresecManager.GetEmployees();
			EmployeeCardIndex.Clear();
			if (list != null)
				foreach (var employee in list)
					EmployeeCardIndex.Add(new EmployeeCardViewModel(employee));

			if (EmployeeCardIndex.Count > 0)
				SelectedEmployeeCard = EmployeeCardIndex[0];
		}

		public override void OnShow()
		{
			ServiceFactory.Layout.ShowMenu(_сardIndexMenuViewModel);
		}
		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);
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
		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowConfirmation(Resources.DeleteEmployeeConfirmation) == MessageBoxResult.Yes)
			{
				if (FiresecManager.DeleteEmployeeCard(SelectedEmployeeCard.EmployeeCardIndex))
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
			//
		}
		public RelayCommand ClearFilterCommand { get; private set; }
		void OnClearFilter()
		{
			//
		}

		bool CanEditRemove()
		{
			return SelectedEmployeeCard != null;
		}
		bool CanRemoveAll()
		{
			return (EmployeeCardIndex.IsNotNullOrEmpty());
		}

		private void BeginEdit(EmployeeCardViewModel card = null)
		{
			bool isNew = card == null;
			if (isNew)
				card = new EmployeeCardViewModel(null);
			card.Initialize();
			if (ServiceFactory.UserDialogs.ShowModalWindow(card))
			{
				if (isNew)
					EmployeeCardIndex.Add(card);
				SelectedEmployeeCard = card;
			}
		}
	}
}