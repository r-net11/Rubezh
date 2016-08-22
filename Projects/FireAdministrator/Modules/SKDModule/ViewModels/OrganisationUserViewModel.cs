using System;
using System.Collections.Generic;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationUserViewModel : BaseViewModel, IOrganisationItemViewModel
	{
		public Organisation Organisation { get; private set; }
		public User User { get; private set; }

		public OrganisationUserViewModel(Organisation organisation, User user)
		{
			Organisation = organisation;
			User = user;
			if (Organisation != null)
			{
				if (Organisation.UserUIDs == null)
					Organisation.UserUIDs = new List<Guid>();
			}
			_isChecked = Organisation != null && Organisation.UserUIDs != null && Organisation.UserUIDs.Contains(user.UID);
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
					if (!Organisation.UserUIDs.Contains(User.UID))
						Organisation.UserUIDs.Add(User.UID);
				}
				else
				{
					if (Organisation.UserUIDs.Contains(User.UID))
						Organisation.UserUIDs.Remove(User.UID);
				}
				OrganisationHelper.SaveUsers(Organisation);
			}
		}

		public void SetWithoutSave(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}
	}
}