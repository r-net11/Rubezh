using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationZoneViewModel : BaseViewModel, IOrganisationItemViewModel
	{
		public Organisation Organisation { get; private set; }
		public SKDZone Zone { get; private set; }

		public OrganisationZoneViewModel(Organisation organisation, SKDZone zone)
		{
			Organisation = organisation;
			Zone = zone;
			_isChecked = Organisation != null && Organisation.ZoneUIDs.Contains(Zone.UID);
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
					if (!Organisation.ZoneUIDs.Contains(Zone.UID))
						Organisation.ZoneUIDs.Add(Zone.UID);
				}
				else
				{
					if (Organisation.ZoneUIDs.Contains(Zone.UID))
						Organisation.ZoneUIDs.Remove(Zone.UID);
				}

				var saveResult = OrganisationHelper.SaveZones(Organisation);
			}
		}
	}
}
