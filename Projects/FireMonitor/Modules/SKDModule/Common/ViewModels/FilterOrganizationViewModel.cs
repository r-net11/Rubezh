using RubezhAPI.SKD;
using Infrastructure.Common.CheckBoxList;

namespace SKDModule.ViewModels
{
	public class FilterOrganisationViewModel : CheckBoxItemViewModel
	{
		public Organisation Organisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public bool IsDeleted { get; private set; }

		public FilterOrganisationViewModel(Organisation organisation)
		{
			Organisation = organisation;
			Name = organisation.Name;
			Description = organisation.Description;
			IsDeleted = organisation.IsDeleted;
		}
	}
}