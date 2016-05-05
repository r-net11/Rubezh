using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
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
