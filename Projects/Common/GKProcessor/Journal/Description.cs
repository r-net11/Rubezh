using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace GKProcessor
{
	public class Description
	{
		public string Name { get; set; }
		public DescriptionType DescriptionType { get; set; }

		public Description(string name, DescriptionType descriptionType)
		{
			Name = name;
			DescriptionType = descriptionType;
		}
	}
}
