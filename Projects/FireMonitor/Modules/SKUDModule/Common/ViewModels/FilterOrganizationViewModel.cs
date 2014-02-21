using Infrastructure.Common.CheckBoxList;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class FilterOrganizationViewModel : CheckBoxItemViewModel
	{
		public FilterOrganizationViewModel(Organization organization)
		{
			Name = organization.Name;
			Organization = organization;
		}

		public string Name { get; private set; }
		public Organization Organization { get; private set; }
	}
}