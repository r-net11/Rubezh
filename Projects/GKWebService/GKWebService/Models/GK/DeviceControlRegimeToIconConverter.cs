using System;
using System.Windows.Data;
using GKWebService.Models.GK;

namespace GKWebService.Converters
{
	public class DeviceControlRegimeToIconConverter
	{
		public string Convert(DeviceControlRegime deviceControlRegime)
		{
			switch (deviceControlRegime)
			{
				case DeviceControlRegime.Automatic:
					return "TechnologicalRegime";
				case DeviceControlRegime.Manual:
					return "Manual";
				case DeviceControlRegime.Ignore:
					return "Ignore";
			}
			return "";
		}
	}
}