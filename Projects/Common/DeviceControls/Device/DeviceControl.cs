using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Devices;

namespace DeviceControls.Device
{
	public class DeviceControl : BaseDeviceControl<LibraryFrame, StateType>
	{
		public StateType StateType { get; set; }
		public List<string> AdditionalStateCodes { get; set; }

		protected override IEnumerable<ILibraryState<LibraryFrame, StateType>> GetStates()
		{
			var libraryDevice = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == DriverId);
			if (libraryDevice == null)
			{
				Logger.Error("DeviceControl.Update libraryDevice = null " + DriverId.ToString());
				return null;
			}

			var additionalLibraryStates = new List<LibraryState>();
			foreach (var additionalStateCode in AdditionalStateCodes)
			{
				var additionalState = libraryDevice.States.FirstOrDefault(x => x.Code == additionalStateCode);
				if (additionalState != null && additionalState.StateType == StateType)
					additionalLibraryStates.Add(additionalState);
			}

			var libraryState = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType);
			if (libraryState == null)
			{
				if (!additionalLibraryStates.Any(x => x.StateType == StateType))
				{
					libraryState = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType.No);
					if (libraryState == null)
					{
						Logger.Error("DeviceControl.Update libraryState = null " + DriverId.ToString());
						return null;
					}
				}
			}

			var resultLibraryStates = new List<ILibraryState<LibraryFrame, StateType>>();
			if (libraryState != null)
				resultLibraryStates.Add(libraryState);
			foreach (var additionalLibraryState in additionalLibraryStates)
				resultLibraryStates.Add(additionalLibraryState);
			return resultLibraryStates;
		}
	}
}