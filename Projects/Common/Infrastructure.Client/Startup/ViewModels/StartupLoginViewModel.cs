using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Client.Properties;
using StrazhAPI.Models;
using Infrastructure.Client.Login.ViewModels;
using System.Threading;

namespace Infrastructure.Client.Startup.ViewModels
{
	internal class StartupLoginViewModel : LoginViewModel
	{
		public StartupLoginViewModel(ClientType clientType)
			: base(clientType)
		{
		}

		protected override bool Save()
		{
			return true;
		}
		protected override bool Cancel()
		{
			return false;
		}
	}
}
