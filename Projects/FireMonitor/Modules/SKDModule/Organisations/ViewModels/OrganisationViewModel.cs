using RubezhAPI.SKD;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationViewModel : BaseViewModel
	{
		public Organisation Organisation { get; set; }

		public OrganisationViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsDeleted = organisation.IsDeleted;
		}

		public void Update()
		{
			OnPropertyChanged(() => Organisation);
		}

		bool _isDeleted;
		public bool IsDeleted
		{
			get { return _isDeleted; }
			set
			{
				_isDeleted = value;
				Organisation.IsDeleted = value;
				OnPropertyChanged(() => IsDeleted);
			}
		}
	}
}