using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModel : OrganisationElementViewModel<EmployeeViewModel, ShortEmployee>
	{
		public string CredentialsStartDateString
		{
			get { return IsOrganisation ? "" : Model.CredentialsStartDate; }
		}
		public string DepartmentName
		{
			get 
			{
				var isWithDeleted = (ParentViewModel as EmployeesViewModel).IsWithDeleted;
				return !IsOrganisation && (isWithDeleted || !IsDepartmentDeleted) ? Model.DepartmentName : "";
			}
		}
		public string PositionName
		{
			get 
			{
				var isWithDeleted = (ParentViewModel as EmployeesViewModel).IsWithDeleted;
				return !IsOrganisation && (isWithDeleted || !IsPositionDeleted) ? Model.PositionName : "";
			}
		}
		public string FirstName
		{
			get { return IsOrganisation ? "" : Model.FirstName; }
		}
		public string SecondName
		{
			get { return IsOrganisation ? "" : Model.SecondName; }
		}
		public string LastName
		{
			get { return IsOrganisation ? "" : Model.LastName; }
		}
		public string Phone
		{
			get { return IsOrganisation ? "" : Model.Phone; }
		}
		public string OrganisationName
		{
			get { return IsOrganisation ? "" : Model.OrganisationName; }
		}
		public bool IsDepartmentDeleted
		{
			get { return IsOrganisation ? false : Model.IsDepartmentDeleted || IsOrganisationDeleted; }
		}
		public bool IsPositionDeleted
		{
			get { return IsOrganisation ? false : Model.IsPositionDeleted || IsOrganisationDeleted; }
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
			
			Update();
		}

		public void InitializeCards()
		{
			EmployeeCardsViewModel = new EmployeeCardsViewModel(this);
		}

		public EmployeeCardsViewModel EmployeeCardsViewModel { get; set; }
		
		
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
			OnPropertyChanged(() => FirstName);
			OnPropertyChanged(() => SecondName);
			OnPropertyChanged(() => LastName);
			OnPropertyChanged(() => OrganisationName);
			OnPropertyChanged(() => Phone);
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => DepartmentName);
			OnPropertyChanged(() => PositionName);
			OnPropertyChanged(() => CredentialsStartDateString);
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

		public bool IsGuest { get { return !IsOrganisation && Model.Type == PersonType.Guest; } }

		public PersonType PersonType
		{
			get { return (ParentViewModel as EmployeesViewModel).PersonType; }
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

		public IEnumerable<SKDCard> Cards { get { return EmployeeCardsViewModel != null ? EmployeeCardsViewModel.Cards.Select(x => x.Card) : new List<SKDCard>(); } }
	}
}