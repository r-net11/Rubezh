using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Infrastructure.Common.Navigation
{
	public class NavigationItem
	{
		public string Title { get; private set; }
		public string Icon { get; private set; }
		public ReadOnlyCollection<NavigationItem> Childs { get; private set; }

		public NavigationItem(string title, string icon = null, IList<NavigationItem> childs = null)
		{
			Title = title;
			Icon = icon;
			Childs = new ReadOnlyCollection<NavigationItem>(childs ?? new List<NavigationItem>());
		}
		public virtual void Execute()
		{
		}
	}

}
