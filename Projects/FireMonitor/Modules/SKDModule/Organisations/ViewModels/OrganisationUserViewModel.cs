using System;
using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.Windows.ViewModels;
using SKDModule.Events;
using RubezhClient;

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
				var userUids = Organisation.UserUIDs;
				if (value)
				{
					if (!userUids.Contains(User.UID))
						userUids.Add(User.UID);
				}
				else
				{
					if (userUids.Contains(User.UID))
						userUids.Remove(User.UID);
				}
				var saveResult = OrganisationHelper.SaveUsers(Organisation);
				if (saveResult)
				{
					Organisation.UserUIDs = userUids;
					ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Publish(Organisation);
					_isChecked = value;
					OnPropertyChanged(() => IsChecked);
				}
			}
		}

		public void SetWithoutSave(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}

		public bool CanChange { get { return User.UID != ClientManager.CurrentUser.UID; } }
	}
}