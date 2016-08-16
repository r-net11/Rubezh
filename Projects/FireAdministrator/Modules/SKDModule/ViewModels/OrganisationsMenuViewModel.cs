using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationsMenuViewModel : BaseViewModel
	{
		public OrganisationsMenuViewModel(OrganisationsViewModel context)
		{
			Context = context;
		}

		public OrganisationsViewModel Context { get; private set; }
	}
}