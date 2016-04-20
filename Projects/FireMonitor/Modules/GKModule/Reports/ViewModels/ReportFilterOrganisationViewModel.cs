using RubezhAPI.SKD;
using Infrastructure.Common.Windows.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ReportFilterOrganisationViewModel : CheckBoxItemViewModel
	{
		public Organisation Organisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public bool IsDeleted { get; private set; }

		public ReportFilterOrganisationViewModel(Organisation organisation)
		{
			Organisation = organisation;
			Name = organisation.Name;
			Description = organisation.Description;
			IsDeleted = organisation.IsDeleted;
		}
	}
}