using RubezhAPI.GK;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace RubezhService.Models
{
	public class GKLifecycleViewModel : BaseViewModel
	{
		public GKLifecycleInfo GKLifecycleInfo { get; private set; }

		public GKLifecycleViewModel(GKLifecycleInfo gkLifecycleInfo)
		{
			GKLifecycleInfo = gkLifecycleInfo;
			Update(gkLifecycleInfo);
		}

		public void Update(GKLifecycleInfo gkLifecycleInfo)
		{
			Time = System.DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");
			if (gkLifecycleInfo.Device.DriverType == GKDriverType.GK)
			{
				Address = gkLifecycleInfo.Device.PresentationAddress;
			}
			else if (gkLifecycleInfo.Device.DriverType == GKDriverType.RSR2_KAU)
			{
				Address = gkLifecycleInfo.Device.Parent.PresentationAddress + " КАУ " + gkLifecycleInfo.Device.PresentationAddress;
			}
			Name = gkLifecycleInfo.Name;
			Progress = gkLifecycleInfo.Progress;
			Items = gkLifecycleInfo.DetalisationItems;
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _progress;
		public string Progress
		{
			get { return _progress; }
			set
			{
				_progress = value;
				OnPropertyChanged(() => Progress);
			}
		}

		string _time;
		public string Time
		{
			get { return _time; }
			set
			{
				_time = value;
				OnPropertyChanged(() => Time);
			}
		}

		List<string> _items;
		public List<string> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged(() => Items);
			}
		}
	}
}