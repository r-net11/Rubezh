using Controls.Menu.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagnosticsModule.ViewModels
{
	public class ZonesMenuViewModel : MenuViewModel
	{
		public ZonesMenuViewModel(ZonesViewModel context)
		{
			Context = context;
		}
		public ZonesViewModel Context { get; private set; }
	}
}