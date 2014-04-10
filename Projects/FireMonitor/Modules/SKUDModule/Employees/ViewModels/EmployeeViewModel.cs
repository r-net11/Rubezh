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
		public Organization Organization { get; private set; }

		public EmployeeViewModel(Organization organization, EmployeeListItem employee)
		{
			Organization = organization;
			EmployeeListItem = employee;

			AddCardCommand = new RelayCommand(OnAddCard);
			SelectEmployeeCommand = new RelayCommand(OnSelectEmployee);

			//DepartmentPhotoUID = department == null ? null : department.PhotoUID;
			//PositionPhotoUID = null; // пока нет в БД - position == null ? null : position.PhotoUID;
			
			Cards = new ObservableCollection<EmployeeCardViewModel>();
			foreach (var item in employee.Cards)
				Cards.Add(new EmployeeCardViewModel(Organization, this, item));
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

		#region Cards
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
			if (Cards.Count > 10)
			{
				MessageBoxService.ShowWarning("У сотрудника не может быть больше 10 карт");
				return;
			}
			var cardDetailsViewModel = new EmployeeCardDetailsViewModel(Organization);
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				card.HolderUID = EmployeeListItem.UID;
				var saveResult = CardHelper.Save(card);
				if (!saveResult)
					return;
				var cardViewModel = new EmployeeCardViewModel(Organization, this, card);
				Cards.Add(cardViewModel);
				SelectedCard = cardViewModel;
				SelectCard(cardViewModel);
			}
		}

		bool _isEmployeeSelected = true;
		public bool IsEmployeeSelected
		{
			get { return _isEmployeeSelected; }
			set
			{
				_isEmployeeSelected = value;
				OnPropertyChanged("IsEmployeeSelected");
			}
		}

		public RelayCommand SelectEmployeeCommand { get; private set; }
		public void OnSelectEmployee()
		{
			IsEmployeeSelected = true;
			foreach (var card in Cards)
			{
				card.IsCardSelected = false;
			}
		}

		public void SelectCard(EmployeeCardViewModel employeeCardViewModel)
		{
			IsEmployeeSelected = false;
			foreach (var card in Cards)
			{
				card.IsCardSelected = false;
			}
			employeeCardViewModel.IsCardSelected = true;
			SelectedCard = employeeCardViewModel;
		}
		#endregion
	}
}