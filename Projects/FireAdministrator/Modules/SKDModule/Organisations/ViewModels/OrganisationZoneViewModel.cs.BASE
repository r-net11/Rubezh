using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using Infrastructure.Common.TreeList;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class OrganisationZoneViewModel : TreeNodeViewModel<OrganisationZoneViewModel>
	{
		public Organization Organization { get; private set; }
		public SKDZone Zone { get; private set; }

		public OrganisationZoneViewModel(Organization organization, SKDZone zone)
		{
			Organization = organization;
			Zone = zone;
			_isChecked = Organization != null && Organization.ZoneUIDs.Contains(zone.UID);
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");

				if (value)
				{
					if (!Organization.ZoneUIDs.Contains(Zone.UID))
						Organization.ZoneUIDs.Add(Zone.UID);
				}
				else
				{
					if (Organization.ZoneUIDs.Contains(Zone.UID))
						Organization.ZoneUIDs.Remove(Zone.UID);
				}

				if (value && Parent != null)
					Parent.IsChecked = true;
				var saveResult = OrganizationHelper.SaveZones(Organization);
			}
		}
	}
}