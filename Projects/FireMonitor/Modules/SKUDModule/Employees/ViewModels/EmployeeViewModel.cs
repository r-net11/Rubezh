using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : BaseViewModel
	{
		public EmployeeViewModel(Employee employee)
		{
			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
			ChangeIsExpandedCommand = new RelayCommand(OnChangeIsExpanded);

			Employee = employee;
			var departmentUID = !Employee.IsReplaced ? Employee.DepartmentUID : Employee.CurrentReplacement.DepartmentUID;
			var department = DepartmentHelper.GetSingle(departmentUID);
			DepartmentName = department != null ? department.Name : "";
			var position = PositionHelper.GetSingle(Employee.PositionUID);
			PositionName = position != null ? position.Name : "";
			AppointedString = Employee.Appointed.ToString("d MMM yyyy");
			DismissedString = Employee.Dismissed.ToString("d MMM yyyy");


			var filter = new CardFilter{ EmployeeUIDs = new List<Guid>() { Employee.UID } };
			Cards = new ObservableCollection<EmployeeCardViewModel>();
			var cards = CardHelper.Get(filter);
			if (cards != null)
			{
				foreach (var item in cards)
					Cards.Add(new EmployeeCardViewModel(this, item));
			}
		}

		public Employee Employee { get; set; }
		public string DepartmentName { get; set; }
		public string PositionName { get; set; }
		public string AppointedString { get; set; }
		public string DismissedString { get; set; }

		public void Update(Employee employee)
		{
			Employee = employee;
			OnPropertyChanged(()=>Employee);
			OnPropertyChanged(()=>DepartmentName);
			OnPropertyChanged(()=>PositionName);
			OnPropertyChanged(()=>AppointedString);
			OnPropertyChanged(()=>DismissedString);
		}

		public ObservableCollection<EmployeeCardViewModel> Cards { get; private set; }

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			var cardDetailsViewModel = new EmployeeCardDetailsViewModel();
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				card.HolderUID = Employee.UID;
				var saveResult = CardHelper.Save(card);
				if (!saveResult)
					return;
				var cardViewModel = new EmployeeCardViewModel(this, card);
				Cards.Add(cardViewModel);

				IsExpanded = true;
				//cardViewModel.SelectCardCommand.Execute();
				EmployeesViewModel.Current.SelectedCard = cardViewModel;
				IsExpanded = true;
			}
		}
		public bool CanAddCard()
		{ 
			return Cards.Count < 10;
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