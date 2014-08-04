using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationViewModel : BaseViewModel
	{
		public Organisation Organisation { get; set; }

		public OrganisationViewModel(Organisation organisation)
		{
			Organisation = organisation;
		}

		public void Update()
		{
			OnPropertyChanged(() => Organisation);
		}
	}
}