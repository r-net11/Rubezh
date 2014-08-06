using System;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DepartmentDetailsViewModel : SaveCancelDialogViewModel
	{
		Guid OrganisationUID { get; set; }
		Department Department { get; set; }

		public DepartmentDetailsViewModel(Guid organisationUID, Guid? departmentUID = null, Guid? parentDepartmentUID = null)
		{
			OrganisationUID = organisationUID;

			if (departmentUID == null)
			{
				Title = "Создание отдела";
				Department = new Department()
				{
					Name = "Новый отдел",
					ParentDepartmentUID = parentDepartmentUID,
					OrganisationUID = OrganisationUID
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
			if (Department.Photo == null)
				Department.Photo = new Photo();
			Department.Photo.Data = PhotoData;
			return DepartmentHelper.Save(Department);
		}
	}
}