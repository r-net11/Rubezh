using System.Net;
using Common;
using FiresecAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using RviCommonClient;
using RviIntegratorClient;
using RviOperatorClient;

namespace RviClient
{
	public static class RviClientHelper
	{
		private static object _rviClientLocker = new object();
		private static IRviClient _rviClient;

		private static IRviClient GetRviClient(VideoIntegrationProvider videoIntegrationProvider)
		{
			lock (_rviClientLocker)
			{
				if (_rviClient == null
					|| (_rviClient.GetType() == typeof(RviOperatorIntegrationClient) && videoIntegrationProvider == VideoIntegrationProvider.RviIntegrator)
					|| (_rviClient.GetType() == typeof(RviIntegratorIntegrationClient) && videoIntegrationProvider == VideoIntegrationProvider.RviOperator))
				{
					switch (videoIntegrationProvider)
					{
						case VideoIntegrationProvider.RviOperator:
							_rviClient = new RviOperatorIntegrationClient();
							break;
						case VideoIntegrationProvider.RviIntegrator:
							_rviClient = new RviIntegratorIntegrationClient();
							break;
					}
				}
			}
			return _rviClient;
		}

		public static List<IRviDevice> GetDevices(SystemConfiguration systemConfiguration)
		{
			return GetRviClient(systemConfiguration.RviSettings.VideoIntegrationProvider).GetDevices(systemConfiguration);
		}

		public static void GetSnapshot(SystemConfiguration systemConfiguration, Camera camera)
		{
			GetRviClient(systemConfiguration.RviSettings.VideoIntegrationProvider).GetSnapshot(systemConfiguration, camera);
		}

		public static void SetPtzPreset(SystemConfiguration systemConfiguration, Camera camera, int number)
		{
			GetRviClient(systemConfiguration.RviSettings.VideoIntegrationProvider).SetPtzPreset(systemConfiguration, camera, number);
		}

		public static void VideoRecordStart(SystemConfiguration systemConfiguration, Camera camera, Guid eventUID, int timeout)
		{
			GetRviClient(systemConfiguration.RviSettings.VideoIntegrationProvider).VideoRecordStart(systemConfiguration, camera, eventUID, timeout);
		}

		public static void VideoRecordStop(SystemConfiguration systemConfiguration, Camera camera, Guid eventUID)
		{
			GetRviClient(systemConfiguration.RviSettings.VideoIntegrationProvider).VideoRecordStop(systemConfiguration, camera, eventUID);
		}

		public static void GetVideoFile(SystemConfiguration systemConfiguration, Guid eventUID, Guid cameraUid, string videoPath)
		{
			GetRviClient(systemConfiguration.RviSettings.VideoIntegrationProvider).GetVideoFile(systemConfiguration, eventUID, cameraUid, videoPath);
		}

		public static void AlarmRuleExecute(SystemConfiguration systemConfiguration, string ruleName)
		{
			GetRviClient(systemConfiguration.RviSettings.VideoIntegrationProvider).AlarmRuleExecute(systemConfiguration, ruleName);
		}

		public static bool PrepareToTranslation(SystemConfiguration systemConfiguration, Camera camera, out IPEndPoint ipEndPoint, out int vendorId)
		{
			return GetRviClient(systemConfiguration.RviSettings.VideoIntegrationProvider).PrepareToTranslation(systemConfiguration, camera, out ipEndPoint, out vendorId);
		}
	}
}