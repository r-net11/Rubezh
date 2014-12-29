using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient.Itv;
using ItvIntergation.Ngi;

namespace RepFileManager
{
	public class ComputerDevice
	{
		public repositoryModuleDevice Device { get; private set; }

		public ComputerDevice()
		{
			Device = new repositoryModuleDevice()
			{
				id = "PanelDevice",
				rootSpecified = true,
				root = true
			};

			CreateChildren();
			CreateStates();
			CreateImages();
			CreateEvents();
			CreateProperties();
			CreateButtons();
		}

		void CreateChildren()
		{
			var children = new List<repositoryModuleDeviceChild>();
			var childDevice = new repositoryModuleDeviceChild()
			{
				id = "CommunicationDevice"
			};
			children.Add(childDevice);
			Device.children = children.ToArray();
		}

		void CreateStates()
		{
			var deviceStates = new List<repositoryModuleDeviceState>();
			foreach (var stateType in Helper.AllStates)
			{
				var deviceState = new repositoryModuleDeviceState()
				{
					id = stateType.ToString(),
					image = "NoImage"
				};
				deviceStates.Add(deviceState);
			}
			Device.states = deviceStates.ToArray();
		}

		void CreateImages()
		{
			// create image NoImage.bmp
		}

		void CreateEvents()
		{
			var driver = ItvManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer);

			var deviceEvents = new List<repositoryModuleDeviceEvent>();
			foreach (var driverState in driver.States)
			{
				if (driverState.Code.StartsWith("Reserved_"))
					continue;

				var deviceEvent = new repositoryModuleDeviceEvent()
				{
					id = driverState.Code
				};
				deviceEvents.Add(deviceEvent);
			}

			Device.events = deviceEvents.ToArray();
		}

		void CreateProperties()
		{
			var driver = ItvManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer);

			var allProperties = Helper.CreateProperties(driver.Properties);
			if (allProperties.Count > 0)
			{
				Device.properties = allProperties.ToArray();
			}
		}

		void CreateButtons()
		{
			var buttons = new List<repositoryModuleDeviceButton>();
			var button = new repositoryModuleDeviceButton()
			{
				id = "LoadConfiguration",
				Value = null
			};
			buttons.Add(button);
			Device.buttons = buttons.ToArray();
		}
	}
}