using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Navigation;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure
{
	public class NavigationItem<T> : NavigationItem
		where T : CompositePresentationEvent<object>, new()
	{
		public NavigationItem(string title, string icon = null, IList<NavigationItem> childs = null)
			: base(title, icon, childs)
		{
		}
		public override void Execute()
		{
			ServiceFactory.Events.GetEvent<T>().Publish(null);
		}
	}
}
