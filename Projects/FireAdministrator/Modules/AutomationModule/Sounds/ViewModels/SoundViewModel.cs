using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SoundViewModel : BaseViewModel
	{
		public const string DefaultName = "<нет>";
		public Sound Sound { get; set; }

		public SoundViewModel(Sound sound)
		{
			Sound = sound;
		}

		public XStateClass StateClass
		{
			get { return Sound.StateClass; }
		}

		public string Name
		{
			get { return Sound.SoundName; }
			set
			{
				Sound.SoundName = value;
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
	}
}