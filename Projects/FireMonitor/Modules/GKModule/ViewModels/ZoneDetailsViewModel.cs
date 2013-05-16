using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecClient;
using FiresecAPI.Models;

namespace GKModule.ViewModels
{
	public class ZoneDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		Guid _guid;
		public XZone Zone { get; private set; }
		public XZoneState ZoneState { get; private set; }

		public ZoneDetailsViewModel(XZone zone)
		{
			_guid = zone.UID;
			Zone = zone;
			ZoneState = Zone.ZoneState;
			ZoneState.StateChanged += new Action(OnStateChanged);

			Title = Zone.PresentationName;
			TopMost = true;
		}

		void OnStateChanged()
		{
			var stateClass = ZoneState.StateClass;
			OnPropertyChanged("ZoneState");
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return _guid.ToString(); }
		}
		#endregion
	}
}