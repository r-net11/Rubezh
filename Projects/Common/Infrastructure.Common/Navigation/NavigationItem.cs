using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Infrastructure.Common.Navigation
{
	public class NavigationItem
	{
		public string Title { get; private set; }
		public string Icon { get; private set; }
		public Type EventType { get; private set; }
		public ReadOnlyCollection<NavigationItem> Childs { get; private set; }

		public NavigationItem(string title, string icon = null,Type eventType = null, IList<NavigationItem> childs = null)
		{
			Title = title;
			Icon = icon;
			EventType = eventType;
			Childs = new ReadOnlyCollection<NavigationItem>(childs ?? new List<NavigationItem>());
		}
	}
}
