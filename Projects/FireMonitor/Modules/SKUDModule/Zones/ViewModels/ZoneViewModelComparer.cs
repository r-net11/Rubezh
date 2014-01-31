using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class ZoneViewModelNameComparer : TreeNodeComparer<ZoneViewModel>
	{
		protected override int Compare(ZoneViewModel x, ZoneViewModel y)
		{
			return string.Compare(x.Zone.Name, y.Zone.Name);
		}
	}
	public class ZoneViewModelDescriptionComparer : TreeNodeComparer<ZoneViewModel>
	{
		protected override int Compare(ZoneViewModel x, ZoneViewModel y)
		{
			return string.Compare(x.Zone.Description, y.Zone.Description);
		}
	}
}