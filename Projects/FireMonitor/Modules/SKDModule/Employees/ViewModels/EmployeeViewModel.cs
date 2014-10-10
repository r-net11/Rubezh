using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : OrganisationElementViewModel<EmployeeViewModel, ShortEmployee>
	{
		public string AppointedString
		{
			get { return IsOrganisation ? "" : Model.Appointed; }
		}
		public string DepartmentName
		{
			get { return IsOrganisation ? "" : Model.DepartmentName; }
		}
		public string PositionName
		{
			get { return IsOrganisation ? "" : Model.PositionName; }
		}
		public bool IsDepartmentDeleted
		{
			get { return IsOrganisation ? false : Model.IsDepartmentDeleted; }
		}
		public bool IsPositionDeleted
		{
			get { return IsOrganisation ? false : Model.IsPositionDeleted; }
		}

		public override void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			base.InitializeOrganisation(organisation, parentViewModel);
		}

		public string[] AdditionalColumnValues 
		{ 
			get
			{
				var additionalColumnTypes = (ParentViewModel as EmployeesViewModel).AdditionalColumnTypes;
				var result = new string[additionalColumnTypes.Count];
				if (this.IsOrganisation)
					return result;
				var i = 0;
				foreach (var additionalColumnType in additionalColumnTypes)
				{
					var textColumn = Model.TextColumns.FirstOrDefault(x => x.ColumnTypeUID == additionalColumnType.UID);
					var columnValue = textColumn != null ? textColumn.Text : "";
					result[i] = columnValue;
					i++;
				}
				return result;
			}
		}

		public override void InitializeModel(Organisation organisation, ShortEmployee model, ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
			AddCardCommand = new RelayCommand(OnAddCard);
			SelectEmployeeCommand = new RelayCommand(OnSelectEmployee);
			InitializeCards();
		}

		private void InitializeCards()
		{
			Cards = new ObservableCollection<EmployeeCardViewModel>();
			foreach (var item in Model.Cards)
				Cards.Add(new EmployeeCardViewModel(Organisation, this, item));
			SelectedCard = Cards.FirstOrDefault();
			Update();
		}
		
		public PhotoColumnViewModel Photo { get; private set; }

		public void UpdatePhoto()
		{
			Photo photo = null;
			if (IsOrganisation)
			{
				var details = OrganisationHelper.GetDetails(Organisation.UID);
				if (details != null)
					photo = details.Photo;
			}
			else
			{
				var details = EmployeeHelper.GetDetails(Model.UID);
				if (details != null)
					photo = details.Photo;
			}
			Photo = new PhotoColumnViewModel(photo);
			OnPropertyChanged(() => Photo);
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => DepartmentName);
			OnPropertyChanged(() => PositionName);
			OnPropertyChanged(() => AppointedString);
			OnPropertyChanged(() => AdditionalColumnValues);
			OnPropertyChanged(() => IsDepartmentDeleted);
			OnPropertyChanged(() => IsPositionDeleted);
			if (IsOrganisation)
			{
				foreach (var child in Children)
				{
					child.Update();
				}
			}
		}

		#region Cards
		public ObservableCollection<EmployeeCardViewModel> Cards { get; private set; }

		EmployeeCardViewModel _selectedCard;
		public EmployeeCardViewModel SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged(() => SelectedCard);
			}
		}

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			if (Cards.Count > 100)
			{
				MessageBoxService.ShowWarning2("У сотрудника не может быть более 100 пропусков");
				return;
			}
			var cardDetailsViewModel = new EmployeeCardDetailsViewModel(Organisation);
			if (DialogService.ShowModalWindow(cardDetailsViewModel))
			{
				var card = cardDetailsViewModel.Card;
				card.HolderUID = Model.UID;
				var saveResult = CardHelper.Add(card);
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
				OnPropertyChanged(() => IsEmployeeSelected);
			}
		}

		bool _isCardSelected;
		public bool IsCardSelected
		{
			get { return _isCardSelected; }
			set
			{
				_isCardSelected = value;
				OnPropertyChanged(() => IsCardSelected);
			}
		}

		public RelayCommand SelectEmployeeCommand { get; private set; }
		public void OnSelectEmployee()
		{
			IsEmployeeSelected = !IsOrganisation;
			IsCardSelected = false;
			foreach (var card in Cards)
			{
				card.IsCardSelected = false;
			}
		}

		public void SelectCard(EmployeeCardViewModel employeeCardViewModel)
		{
			IsEmployeeSelected = false;
			IsCardSelected = true;
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