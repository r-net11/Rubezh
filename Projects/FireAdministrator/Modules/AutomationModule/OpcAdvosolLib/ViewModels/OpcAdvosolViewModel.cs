using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using OPCDA.NET;
//using OPCDA.Interface;

namespace AutomationModule.ViewModels
{
	public class OpcAdvosolViewModel : MenuViewPartViewModel
	{
		#region Fields And Properties
		OpcServer _selectedServer;

		public OpcServer SelectedServer
		{
			get { return _selectedServer; }
			set 
			{
				_selectedServer = value;
				OnPropertyChanged(() => SelectedServer);
			}
		}

		#endregion

		#region Methods
		public void Initialize()
		{ }

		#endregion
	}
}
