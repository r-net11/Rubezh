using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Common;
using Localization.SKD.Common;
using SKDModule.ViewModels;

namespace SKDModule.Employees.Comparers
{
	public class DoorsComparer : ICustomSorter
	{
		public DoorsComparer()
		{
		}

        private static string Always = CommonResources.Always;
        private static string Never = CommonResources.Never;
		public ListSortDirection SortDirection { get; set; }

		public int Compare(object x, object y)
		{
			var firstModel = x as ReadOnlyAccessDoorViewModel;
			var secondModel = y as ReadOnlyAccessDoorViewModel;
			if (firstModel == null || secondModel == null) return 0;

			if (string.Equals(firstModel.EnerScheduleName, secondModel.EnerScheduleName)) return 0;

			if (SortDirection == ListSortDirection.Ascending)
			{
				if (string.Equals(firstModel.EnerScheduleName, Always) && string.Equals(secondModel.EnerScheduleName, Never)) return -1;
				if (string.Equals(firstModel.EnerScheduleName, Never) && string.Equals(secondModel.EnerScheduleName, Always)) return 1;
			}

			if (SortDirection == ListSortDirection.Descending)
			{
				if (string.Equals(firstModel.EnerScheduleName, Always) && string.Equals(secondModel.EnerScheduleName, Never)) return 1;
				if (string.Equals(firstModel.EnerScheduleName, Never) && string.Equals(secondModel.EnerScheduleName, Always)) return -1;
			}

			return string.Compare(firstModel.EnerScheduleName, secondModel.EnerScheduleName);
		}
	}
}
