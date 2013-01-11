using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using Common;
using Firesec.Models.Plans;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace Firesec
{
	public partial class ConfigurationConverter
	{
		surfaces ConvertPlansBack(PlansConfiguration plansConfiguration)
		{
			var innerPlans = new surfaces();

			foreach (var plan in plansConfiguration.Plans)
			{
				var caption = plan.Caption;
			}

			return innerPlans;
		}
	}
}