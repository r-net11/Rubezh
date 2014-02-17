using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
    public class OrganisationViewModel : BaseViewModel
    {
		public SKDOrganisation Organisation { get; set; }

		public OrganisationViewModel(SKDOrganisation organisation)
		{
			Organisation = organisation;
		}

		public void Update()
		{
			OnPropertyChanged("Organisation");
		}
    }
}