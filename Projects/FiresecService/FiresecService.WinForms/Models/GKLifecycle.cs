using RubezhAPI.GK;
using GKProcessor;
using System.Collections.Generic;
using System;

namespace FiresecService.Models
{
	public class GKLifecycle: System.ComponentModel.INotifyPropertyChanged
	{
		public GKLifecycle(GKLifecycleInfo gkLifecycleInfo)
		{
			GKLifecycleInfo = gkLifecycleInfo;
			Update(gkLifecycleInfo);
		}

		public GKLifecycleInfo GKLifecycleInfo { get; private set; }
		public string Address { get; set; }
		public string Name { get; set; }

		string _progress;
		public string Progress 
		{ 
			get { return _progress; }
			set 
			{
				_progress = value;
				OnPropertyChanged("Progress");
			}
		}
		public string Time { get; set; }
		public List<string> Items { get; set; }

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

		void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, 
					new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
	}
}