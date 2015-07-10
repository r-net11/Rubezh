using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.Devices
{
	class MirrorUserNewModel:BaseViewModel
	{
		public MirrorUser MirrorUser { get; set; }
		public MirrorUserNewModel(MirrorUser mirrorUser )
		{
			MirrorUser = mirrorUser;

		}
	}
}
