using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationExportDetailsViewModel : BaseViewModel
	{
		public OrganisationExportDetailsViewModel(ExportFilter filter)
		{
			OrganisationUID = filter.OrganisationUID;
			OrganisationName = filter.OrganisationName;
			IsWithDeleted = filter.IsWithDeleted;
			Path = filter.Path;
		}

		public Guid OrganisationUID { get; private set; }
		public string OrganisationName { get; private set; }
		
		bool _IsWithDeleted;
		public bool IsWithDeleted
		{
			get { return _IsWithDeleted; }
			set
			{
				_IsWithDeleted = value;
				OnPropertyChanged(() => IsWithDeleted);
			}
		}

		bool _IsWithDepartments;
		public bool IsWithDepartments
		{
			get { return _IsWithDepartments; }
			set
			{
				_IsWithDepartments = value;
				OnPropertyChanged(() => IsWithDepartments);
			}
		}

		bool _IsWithPositions;
		public bool IsWithPositions
		{
			get { return _IsWithPositions; }
			set
			{
				_IsWithPositions = value;
				OnPropertyChanged(() => IsWithPositions);
			}
		}

		bool _IsWithEmployees;
		public bool IsWithEmployees
		{
			get { return _IsWithEmployees; }
			set
			{
				_IsWithEmployees = value;
				OnPropertyChanged(() => IsWithEmployees);
			}
		}

		bool _IsWithGuests;
		public bool IsWithGuests
		{
			get { return _IsWithGuests; }
			set
			{
				_IsWithGuests = value;
				OnPropertyChanged(() => IsWithGuests);
			}
		}

		string _Path;
		public string Path
		{
			get { return _Path; }
			set
			{
				_Path = value;
				OnPropertyChanged(() => Path);
			}
		}

		public ExportFilter Filter
		{
			get
			{
				return new ExportFilter
				{
					OrganisationUID = OrganisationUID,
					IsWithDeleted = IsWithDeleted,
					OrganisationName = OrganisationName,
					Path = Path
				};
			}
		}
	}
}
