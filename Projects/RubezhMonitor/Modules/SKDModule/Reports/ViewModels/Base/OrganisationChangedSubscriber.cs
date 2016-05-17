using Infrastructure;
using Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDModule
{
	public class OrganisationChangedSubscriber
	{
		IOrganisationItemsFilterPage _parent;

		public OrganisationChangedSubscriber(IOrganisationItemsFilterPage parent)
		{
			_parent = parent;
			ServiceFactory.Events.GetEvent<SKDReportOrganisationChangedEvent>().Unsubscribe(OnOrganisationChanged);
			ServiceFactory.Events.GetEvent<SKDReportOrganisationChangedEvent>().Subscribe(OnOrganisationChanged);
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Subscribe(OnUseArchive);
		}

		void OnUseArchive(bool isWithDeleted)
		{
			_parent.IsWithDeleted = isWithDeleted;
			_parent.InitializeFilter();
		}

		void OnOrganisationChanged(List<Guid> organisationUIDs)
		{
			_parent.OrganisationUIDs = organisationUIDs;
			_parent.InitializeFilter();
		}

		public void Unsubscribe()
		{
			ServiceFactory.Events.GetEvent<SKDReportOrganisationChangedEvent>().Unsubscribe(OnOrganisationChanged);
			ServiceFactory.Events.GetEvent<SKDReportUseArchiveChangedEvent>().Unsubscribe(OnUseArchive);
		}
	}
}
