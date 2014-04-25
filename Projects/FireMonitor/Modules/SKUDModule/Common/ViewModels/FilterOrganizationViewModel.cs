using FiresecAPI;
using Infrastructure.Common.CheckBoxList;

namespace SKDModule.ViewModels
{
	public class FilterOrganisationViewModel : CheckBoxItemViewModel
	{
		public FilterOrganisationViewModel(Organisation organisation)
		{
			Name = organisation.Name;
			Organisation = organisation;
		}

		public string Name { get; private set; }
		public Organisation Organisation { get; private set; }
	}
}