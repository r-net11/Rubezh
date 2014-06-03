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
	public class AccessTemplateGuardZoneViewModel : BaseViewModel
	{
		public AccessTemplate AccessTemplate { get; private set; }
		public XGuardZone GuardZone { get; private set; }

		public AccessTemplateGuardZoneViewModel(AccessTemplate accessTemplate, XGuardZone guardZone)
		{
			AccessTemplate = accessTemplate;
			GuardZone = guardZone;
			if (AccessTemplate != null)
			{
				if (AccessTemplate.GuardZoneAccesses == null)
					AccessTemplate.GuardZoneAccesses = new List<XGuardZoneAccess>();
			}
			IsChecked = AccessTemplate != null && AccessTemplate.GuardZoneAccesses != null && AccessTemplate.GuardZoneAccesses.Any(x => x.ZoneUID == guardZone.UID);
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

		bool _canSetZone;
		public bool CanSetZone
		{
			get { return _canSetZone; }
			set
			{
				_canSetZone = value;
				OnPropertyChanged("CanSetZone");
			}
		}

		bool _canUnSetZone;
		public bool CanUnSetZone
		{
			get { return _canUnSetZone; }
			set
			{
				_canUnSetZone = value;
				OnPropertyChanged("CanUnSetZone");
			}
		}
	}
}