using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
	public class NavigationItem
	{
		public string Title { get; set; }

		public string Icon { get; set; }

		public bool IsVisible { get; set; }

		public string State { get; set; }

		public string Sref { get; set; }

		public NavigationItem()
		{
			
		}

		public NavigationItem(string title, string icon, string state, string sref)
		{
			Title = title;
			Icon = icon;
			State = state;
			Sref = sref;
			IsVisible = true;
		}
	}
}