using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecClient;
using System.Windows.Documents;

namespace GKModule.ViewModels
{
	public class InstructionViewModel : BaseViewModel
	{
		public XInstruction Instruction { get; private set; }

		public InstructionViewModel(XInstruction instruction)
		{
			Instruction = instruction;
		}

		public XInstructionType InstructionType
		{
			get { return Instruction.InstructionType; }
			set
			{
				Instruction.InstructionType = value;
				OnPropertyChanged("InstructionType");
			}
		}

		public string DirectionsString 
		{
			get 
			{
				if (Instruction.Directions == null)
					return "";
				var directions = new List<XDirection>();
				foreach (var uid in Instruction.Directions)
				{
					var direction = XManager.Directions.FirstOrDefault(x => x.UID == uid);
					if (direction != null)
						directions.Add(direction);
				}
				return XManager.GetCommaSeparatedDirections(directions); 
			} 
		}
		public string ZonesString 
		{
			get
			{
				if (Instruction.ZoneUIDs == null)
					return "";
				var zones = new List<XZone>();
				foreach (var uid in Instruction.ZoneUIDs)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.UID == uid);
					if (zone != null)
						zones.Add(zone);
				}
				return XManager.GetCommaSeparatedZones(zones);
			}
		}
		public string DevicesString 
		{
			get
			{
				if (Instruction.Devices == null)
					return "";
				var devices = new List<XDevice>();
				foreach (var uid in Instruction.Devices)
				{
					var device = XManager.Devices.FirstOrDefault(x => x.UID == uid);
					if (device != null)
						devices.Add(device);
				}
				return XManager.GetCommaSeparatedDevices(devices);
			}
		}

		public List<XInstructionType> InstructionTypes
		{
			get { return Enum.GetValues(typeof(XInstructionType)).Cast<XInstructionType>().ToList(); }
		}

		public void Update()
		{
			OnPropertyChanged("Instruction");
			OnPropertyChanged("DevicesString");
			OnPropertyChanged("ZonesString");
			OnPropertyChanged("DirectionsString");
		}
	}
}