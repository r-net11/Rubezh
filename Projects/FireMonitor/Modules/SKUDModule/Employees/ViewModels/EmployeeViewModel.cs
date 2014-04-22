using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }

		public EmployeeViewModel(Organisation organisation, ShortEmployee employee)
		{
			Organisation = organisation;
			ShortEmployee = employee;

			AddCardCommand = new RelayCommand(OnAddCard);
			SelectEmployeeCommand = new RelayCommand(OnSelectEmployee);

			//DepartmentPhotoUID = department == null ? null : department.PhotoUID;
			//PositionPhotoUID = null; // пока нет в БД - position == null ? null : position.PhotoUID;
			
			Cards = new ObservableCollection<EmployeeCardViewModel>();
			foreach (var item in employee.Cards)
				Cards.Add(new EmployeeCardViewModel(Organisation, this, item));
			SelectedCard = Cards.FirstOrDefault();
		}

		public ShortEmployee ShortEmployee { get; set; }
		public Guid? DepartmentPhotoUID { get; set; }
		public Guid? PositionPhotoUID { get; set; }
		public string FirstName 
		{
			get { return ShortEmployee.FirstName; }
		}
		public string SecondName
		{
			get { return ShortEmployee.SecondName; }
		}
		public string LastName
		{
			get { return ShortEmployee.LastName; }
		}
		public string AppointedString
		{
			get { return ShortEmployee.Appointed; }
		}
		public string DismissedString
		{
			get { return ShortEmployee.Dismissed; }
		}
		public string DepartmentName
		{
			get { return ShortEmployee.DepartmentName; }
		}
		public string PositionName
		{
			get { return ShortEmployee.PositionName; }
		}

		public void Update(ShortEmployee employee)
		{
			ShortEmployee = employee;
			OnPropertyChanged(() => ShortEmployee);
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
			var cardDetailsViewModel = new EmployeeCardDetailsViewModel(Organisation);
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				card.HolderUID = ShortEmployee.UID;
				var saveResult = CardHelper.Save(card);
				if (!saveResult)
					return;
				var cardViewModel = new EmployeeCardViewModel(Organisation, this, card);
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