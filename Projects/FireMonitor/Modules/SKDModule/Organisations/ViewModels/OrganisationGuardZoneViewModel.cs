using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationGuardZoneViewModel : BaseViewModel, IOrganisationItemViewModel
	{
		public Organisation Organisation { get; private set; }
		public XGuardZone GuardZone { get; private set; }

		public OrganisationGuardZoneViewModel(Organisation organisation, XGuardZone guardZone)
		{
			Organisation = organisation;
			GuardZone = guardZone;
			if (Organisation != null)
			{
				if (Organisation.GuardZoneUIDs == null)
					Organisation.GuardZoneUIDs = new List<Guid>();
			}
			_isChecked = Organisation != null && Organisation.GuardZoneUIDs != null && Organisation.GuardZoneUIDs.Contains(guardZone.UID);
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				if (value)
				{
					if (!Organisation.GuardZoneUIDs.Contains(GuardZone.UID))
						Organisation.GuardZoneUIDs.Add(GuardZone.UID);
				}
				else
				{
					if (Organisation.GuardZoneUIDs.Contains(GuardZone.UID))
						Organisation.GuardZoneUIDs.Remove(GuardZone.UID);
				}
				var saveResult = OrganisationHelper.SaveGuardZones(Organisation);
			}
		}
	}
}