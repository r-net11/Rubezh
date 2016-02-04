using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecService.Views;

namespace FiresecService.Presenters
{
	public class TabPageConnectionsPresenter: TabPagePresenter
	{
		#region Constructors

		public TabPageConnectionsPresenter(ITabPageConnectionsView view)
		{
			View = view;
		}

		#endregion

		#region Fields And Properties

		public ITabPageConnectionsView View { get; private set; }

		#endregion
	}
}
