using System;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DepartmentDetailsViewModel : SaveCancelDialogViewModel
	{
		Department Department;
		DepartmentsViewModel DepartmentsViewModel;
		Organisation Organisation;

		public DepartmentDetailsViewModel(DepartmentsViewModel departmentsViewModel, Organisation organisation, Guid? departmentUID = null, Guid? parentDepartmentUID = null)
		{
			Organisation = organisation;

			DepartmentsViewModel = departmentsViewModel;
			if (departmentUID == null)
			{
				Title = "Создание отдела";
				Department = new Department()
				{
					Name = "Новый отдел",
					ParentDepartmentUID = parentDepartmentUID,
					OrganisationUID = Organisation.UID
				};
			}
			else
			{
				Department = DepartmentHelper.GetDetails(departmentUID);
				Title = string.Format("Свойства отдела: {0}", Department.Name);
			}
			CopyProperties();
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
					OnPropertyChanged("Name");
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
					OnPropertyChanged("Description");
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

		public ShortDepartment ShortDepartment 
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
			if (PhotoData != null && PhotoData.Length > 0)
			{
				Department.Photo = new Photo();
				Department.Photo.Data = PhotoData;
			}
			return DepartmentHelper.Save(Department);
		}
	}
}