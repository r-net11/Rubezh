using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DepartmentDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortDepartment>
	{
		Guid OrganisationUID { get; set; }
		Department Department { get; set; }
		public ChiefViewModel ChiefViewModel { get; private set; }

		public DepartmentDetailsViewModel() { }
		
		public bool Initialize(Organisation organisation, ShortDepartment shortDepartment, ViewPartViewModel parentViewModel)
		{
			OrganisationUID = organisation.UID;
			if (shortDepartment == null)
			{
				Title = "Создание отдела";
				var parentModel = (parentViewModel as DepartmentsViewModel).SelectedItem.Model;
				Department = new Department()
				{
					Name = "Новый отдел",
					ParentDepartmentUID = parentModel != null ? parentModel.UID : new Guid?(),
					OrganisationUID = OrganisationUID
				};
			}
			else
			{
				Department = DepartmentHelper.GetDetails(shortDepartment.UID);
				Title = string.Format("Свойства отдела: {0}", Department.Name);
			}
			CopyProperties();
			ChiefViewModel = new ChiefViewModel(Department.ChiefUID, new EmployeeFilter { DepartmentUIDs = new List<Guid> { Department.UID } });
			return true;
		}

		public void Initialize(Guid organisationUID, Guid? parentDepartmentUID)
		{
			OrganisationUID = organisationUID;
			Title = "Создание отдела";
			Department = new Department()
			{
				Name = "Новый отдел",
				ParentDepartmentUID = parentDepartmentUID,
				OrganisationUID = OrganisationUID
			};
			CopyProperties();
			ChiefViewModel = new ChiefViewModel(Department.ChiefUID, new EmployeeFilter { DepartmentUIDs = new List<Guid> { Department.UID } });
		}

		public void CopyProperties()
		{
			Name = Department.Name;
			Description = Department.Description;
			if (Department.Photo != null)
				PhotoData = Department.Photo.Data;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(() => Name);
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged(() => Description);
				}
			}
		}

		byte[] _photoData;
		public byte[] PhotoData
		{
			get { return _photoData; }
			set
			{
				_photoData = value;
				OnPropertyChanged(()=>PhotoData);
			}
		}

		protected override bool CanSave()
		{
			return true;
		}

		public ShortDepartment Model 
		{ 
			get
			{
				return new ShortDepartment
				{
					UID = Department.UID,
					Description = Department.Description,
					Name = Department.Name,
					ParentDepartmentUID = Department.ParentDepartmentUID,
					ChildDepartmentUIDs = Department.ChildDepartmentUIDs
				};
			}
		}

		protected override bool Save()
		{
			Department.Name = Name;
			Department.Description = Description;
			if (Department.Photo == null)
				Department.Photo = new Photo();
			Department.Photo.Data = PhotoData;
			Department.ChiefUID = ChiefViewModel.ChiefUID;
			return DepartmentHelper.Save(Department);
		}
	}

	
}