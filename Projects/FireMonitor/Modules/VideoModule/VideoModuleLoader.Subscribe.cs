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
		void InitializeStates()
		{
			var rviStates = ClientManager.FiresecService.GetRviStates();
			CopyRviStates(rviStates);
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
					else if (remoteRviState.RviServerUrl != null)
					{
						var server = ClientManager.SystemConfiguration.RviServers.FirstOrDefault(x => x.Url == remoteRviState.RviServerUrl);
						if (server != null)
						{
							server.Status = remoteRviState.Status;
							server.OnStatusChanged();
						}
					}
					else if (remoteRviState.CameraUid != Guid.Empty)
					{
						var camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == remoteRviState.CameraUid);
						if (camera != null)
						{
							camera.Status = remoteRviState.Status;
							camera.IsOnGuard = remoteRviState.IsOnGuard;
							camera.IsRecordOnline = remoteRviState.IsRecordOnline;
							camera.RviStreams = remoteRviState.RviStreams;
							camera.OnStatusChanged();
						}
					}
				}
			}
		}
	}
}