using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

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

		private bool _isDeleted;
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