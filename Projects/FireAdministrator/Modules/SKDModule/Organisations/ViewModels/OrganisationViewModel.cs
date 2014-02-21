using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
    public class OrganisationViewModel : BaseViewModel
    {
		public Organization Organisation { get; set; }

		public OrganisationViewModel(Organization organisation)
		{
			Organisation = organisation;
		}

		public void Update()
		{
			OnPropertyChanged("Organisation");
		}
    }
}