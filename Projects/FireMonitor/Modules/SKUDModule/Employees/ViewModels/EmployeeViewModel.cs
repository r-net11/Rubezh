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
			Employee = employee;

			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
			ChangeIsExpandedCommand = new RelayCommand(OnChangeIsExpanded);

			DepartmentName = employee.DepartmentName;
			//DepartmentPhotoUID = department == null ? null : department.PhotoUID;
			PositionName = employee.PositionName;
			//PositionPhotoUID = null; // пока нет в БД - position == null ? null : position.PhotoUID;
			
			AppointedString = Employee.Appointed;
			DismissedString = Employee.Dismissed;

			Cards = new ObservableCollection<EmployeeCardViewModel>();
			foreach (var item in employee.Cards)
					Cards.Add(new EmployeeCardViewModel(EmployeesViewModel.Organization, this, item));
		}

		public EmployeeListItem Employee { get; set; }
		public string DepartmentName { get; set; }
		public Guid? DepartmentPhotoUID { get; set; }
		public string PositionName { get; set; }
		public Guid? PositionPhotoUID { get; set; }
		public string AppointedString { get; set; }
		public string DismissedString { get; set; }

		public void Update(EmployeeListItem employee)
		{
			Employee = employee;
			OnPropertyChanged(() => Employee);
			OnPropertyChanged(() => DepartmentName);
			OnPropertyChanged(() => PositionName);
			OnPropertyChanged(() => AppointedString);
			OnPropertyChanged(() => DismissedString);
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
				card.HolderUID = Employee.UID;
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