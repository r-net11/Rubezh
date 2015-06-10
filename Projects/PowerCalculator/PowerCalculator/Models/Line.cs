﻿using System.Collections.Generic;

namespace PowerCalculator.Models
{
	public class Line
	{
		public Line()
		{
			Name = "Название АЛС";
			Devices = new List<Device>();
		}

		public string Name { get; set; }
		public List<Device> Devices { get; set; }
	}
}