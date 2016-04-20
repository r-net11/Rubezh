using RubezhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{	
	public class OrganisationViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }

		public OrganisationViewModel(Organisation organisation)
		{
			Organisation = organisation;
		}
	}
}
