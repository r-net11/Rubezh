using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Events;

namespace SKDModule.ViewModels
{
	public class ReportFilterOrganisationViewModel : CheckBoxItemViewModel
	{
		public Organisation Organisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public bool IsDeleted { get; private set; }

		protected override void Update(bool value)
		{
			if (value)
				ServiceFactory.Events.GetEvent<SKDReportOrganisationChangedEvent>().Publish(Organisation.UID);
			base.Update(value);
		}

		public ReportFilterOrganisationViewModel(Organisation organisation)
		{
			Organisation = organisation;
			Name = organisation.Name;
			Description = organisation.Description;
			IsDeleted = organisation.IsDeleted;
		}
	}
}
