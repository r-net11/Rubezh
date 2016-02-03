using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecService.Views;

namespace FiresecService.Presenters
{
	public class MainPresenter
	{
		public MainPresenter(IMainView view)
		{
			View = view;
		}

		#region Fields And Properties

		public IMainView View { get; private set; }
		
		#endregion
	}
}
