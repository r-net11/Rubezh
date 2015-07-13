using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public class MirrorUserNewModel : BaseViewModel
	{
		public MirrorUser MirrorUser { get; set; }
		public MirrorUserNewModel(MirrorUser mirrorUser)
		{
			MirrorUser = mirrorUser;
		}

		public string Name
		{
			get { return MirrorUser.Name; }	
		}

		public string Password
		{
			get { return MirrorUser.Password; }
		}

		public GKCardType Type
		{
			get { return MirrorUser.Type; }
		}

		public void Update(MirrorUser mirrorUser)
		{
			MirrorUser = mirrorUser;
			OnPropertyChanged(() => MirrorUser.Name);
			OnPropertyChanged(() => MirrorUser.Type);
			OnPropertyChanged(() => MirrorUser.Password);
		
		}

	}
}
