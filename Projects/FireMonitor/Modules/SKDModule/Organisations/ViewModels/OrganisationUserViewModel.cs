using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using FiresecAPI.Models;

namespace SKDModule.ViewModels
{
	public class OrganisationUserViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }
		public User User { get; private set; }

		public OrganisationUserViewModel(Organisation organisation, User user)
		{
			Organisation = organisation;
			User = user;
			_isChecked = Organisation != null && Organisation.ZoneUIDs.Contains(user.UID);
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");

				//if (value)
				//{
				//    if (!Organisation.ZoneUIDs.Contains(User.UID))
				//        Organisation.ZoneUIDs.Add(User.UID);
				//}
				//else
				//{
				//    if (Organisation.ZoneUIDs.Contains(User.UID))
				//        Organisation.ZoneUIDs.Remove(User.UID);
				//}

				//var saveResult = OrganisationHelper.SaveZones(Organisation);
			}
		}
	}
}
