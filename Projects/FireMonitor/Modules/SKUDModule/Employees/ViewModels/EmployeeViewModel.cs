using System;
using System.Linq;
using System.Collections.ObjectModel;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : BaseViewModel
	{
		public EmployeesViewModel EmployeesViewModel { get; private set; }

		public EmployeeViewModel(EmployeesViewModel employeesViewModel, EmployeeListItem employee)
		{
			EmployeesViewModel = employeesViewModel;
			EmployeeListItem = employee;

			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
			ShowEmployeeCommand = new RelayCommand(OnShowEmployee);

			//DepartmentPhotoUID = department == null ? null : department.PhotoUID;
			//PositionPhotoUID = null; // пока нет в БД - position == null ? null : position.PhotoUID;
			
			Cards = new ObservableCollection<EmployeeCardViewModel>();
			foreach (var item in employee.Cards)
				Cards.Add(new EmployeeCardViewModel(EmployeesViewModel.Organization, this, item));
			SelectedCard = Cards.FirstOrDefault();
		}

		public EmployeeListItem EmployeeListItem { get; set; }
		public Guid? DepartmentPhotoUID { get; set; }
		public Guid? PositionPhotoUID { get; set; }
		public string FirstName 
		{
			get { return EmployeeListItem.FirstName; }
		}
		public string SecondName
		{
			get { return EmployeeListItem.SecondName; }
		}
		public string LastName
		{
			get { return EmployeeListItem.LastName; }
		}
		public string AppointedString
		{
			get { return EmployeeListItem.Appointed; }
		}
		public string DismissedString
		{
			get { return EmployeeListItem.Dismissed; }
		}
		public string DepartmentName
		{
			get { return EmployeeListItem.DepartmentName; }
		}
		public string PositionName
		{
			get { return EmployeeListItem.PositionName; }
		}
		

		public void Update(EmployeeListItem employee)
		{
			EmployeeListItem = employee;
			OnPropertyChanged(() => EmployeeListItem);
			OnPropertyChanged(() => DepartmentName);
			OnPropertyChanged(() => PositionName);
			OnPropertyChanged(() => AppointedString);
			OnPropertyChanged(() => DismissedString);
			OnPropertyChanged(() => FirstName);
			OnPropertyChanged(() => SecondName);
			OnPropertyChanged(() => LastName);
		}

		public ObservableCollection<string> AdditionalColumnValues { get; set; }

		public ObservableCollection<EmployeeCardViewModel> Cards { get; private set; }

		EmployeeCardViewModel _selectedCard;
		public EmployeeCardViewModel SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged("SelectedCard");
			}
		}

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			var cardDetailsViewModel = new EmployeeCardDetailsViewModel(EmployeesViewModel.Organization);
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				card.HolderUID = EmployeeListItem.UID;
				var saveResult = CardHelper.Save(card);
				if (!saveResult)
					return;
				var cardViewModel = new EmployeeCardViewModel(EmployeesViewModel.Organization, this, card);
				Cards.Add(cardViewModel);

				EmployeesViewModel.SelectedCard = cardViewModel;
			}
		}
		public bool CanAddCard()
		{
			return Cards.Count < 1000;
		}

		bool _isCard = false;
		public bool IsCard
		{
			get { return _isCard; }
			set
			{
				_isCard = value;
				OnPropertyChanged("IsCard");
			}
		}

		public RelayCommand ShowEmployeeCommand { get; private set; }
		void OnShowEmployee()
		{
			IsCard = false;
		}
	}
}