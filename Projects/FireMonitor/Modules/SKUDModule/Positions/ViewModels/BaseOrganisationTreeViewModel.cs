using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class BaseOrganisationTreeViewModel : TreeNodeViewModel<BaseOrganisationTreeViewModel>
	{
		public Organisation Organisation { get; protected set; }
		public bool IsOrganisation { get; protected set; }
		public string Name { get; protected set; }
		public string Description { get; protected set; }
	}

	public class BaseOrganisationNameComparer : TreeNodeComparer<BaseOrganisationTreeViewModel>
	{
		protected override int Compare(BaseOrganisationTreeViewModel x, BaseOrganisationTreeViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}
}