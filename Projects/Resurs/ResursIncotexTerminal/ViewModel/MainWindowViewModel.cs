using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.Networks;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.OSI.ApplicationLayer;

namespace ResursIncotexTerminal.ViewModel
{
	public class MainWindowViewModel
	{
		#region Fields And Properties
		
		public NetworksManager NetworkManager
		{ 
			get 
			{ return NetworksManager.Instance; } 
		}

		public IList<INetwrokController> Networks
		{
			get
			{
				return NetworksManager.Instance.Networks;
			}
		}

		#endregion

		#region Constructors
		public MainWindowViewModel()
		{
			var controller = new IncotexNetworkControllerVirtual();
			NetworksManager.Instance.Networks.Add(controller);

			var device = new Mercury203Virtual();
			controller.Devices.Add(device);
		}

		#endregion
	}
}
