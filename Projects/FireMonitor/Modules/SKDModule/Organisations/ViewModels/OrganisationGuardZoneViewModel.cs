using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;
using System.Linq;
using FiresecAPI.GK;

namespace SKDModule.ViewModels
{
	public class OrganisationGuardZoneViewModel : BaseViewModel
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
				OnPropertyChanged("IsChecked");
			}
		}
	}
}