using Infrastructure.Common.Windows;
using RubezhAPI;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VideoModule
{
	public partial class VideoModuleLoader
	{
		void RviAfterInitialize()
		{
			SafeFiresecService.RviCallbackResultEvent -= new Action<RviCallbackResult>(OnRviCallbackResult);
			SafeFiresecService.RviCallbackResultEvent += new Action<RviCallbackResult>(OnRviCallbackResult);
		}
		void OnRviCallbackResult(RviCallbackResult rviCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				CopyRviStates(rviCallbackResult.RviStates);
			});
		}
		void CopyRviStates(List<RviState> rviStates)
		{
			{
				foreach (var remoteRviState in rviStates)
				{

					if (remoteRviState.RviDeviceUid != Guid.Empty)
					{
						var device = ClientManager.SystemConfiguration.RviDevices.FirstOrDefault(x => x.Uid == remoteRviState.RviDeviceUid);
						if (device != null)
						{
							device.Status = remoteRviState.Status;
							device.OnStatusChanged();
						}
					}
					if (remoteRviState.RviServerUrl != null)
					{
						var server = ClientManager.SystemConfiguration.RviServers.FirstOrDefault(x => x.Url == remoteRviState.RviServerUrl);
						if (server != null)
						{
							server.Status = remoteRviState.Status;
							server.OnStatusChanged();

						}
					}
				}
			}
		}
	}
}