using System;
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
			ChangeIsExpandedCommand = new RelayCommand(OnChangeIsExpanded);

			//DepartmentPhotoUID = department == null ? null : department.PhotoUID;
			//PositionPhotoUID = null; // пока нет в БД - position == null ? null : position.PhotoUID;
			
			Cards = new ObservableCollection<EmployeeCardViewModel>();
			foreach (var item in employee.Cards)
					Cards.Add(new EmployeeCardViewModel(EmployeesViewModel.Organization, this, item));
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

				IsExpanded = true;
				EmployeesViewModel.SelectedCard = cardViewModel;
			}
		}
		public bool CanAddCard()
		{
			return Cards.Count < 1000;
		}

		bool _isExpanded = false;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				_isExpanded = value;
				OnPropertyChanged("IsExpanded");
			}
		}

		public RelayCommand ChangeIsExpandedCommand { get; private set; }
		void OnChangeIsExpanded()
		{
			IsExpanded = !IsExpanded;
		}
	}
}