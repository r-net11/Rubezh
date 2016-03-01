using System;
using System.Collections.Generic;
using System.Net;
using FiresecAPI.Models;

namespace RviCommonClient
{
	public interface IRviClient
	{
		List<IRviDevice> GetDevices(SystemConfiguration systemConfiguration);

		void GetSnapshot(SystemConfiguration systemConfiguration, Camera camera);

		void SetPtzPreset(SystemConfiguration systemConfiguration, Camera camera, int number);

		void VideoRecordStart(SystemConfiguration systemConfiguration, Camera camera, Guid eventUID, int timeout);

		void VideoRecordStop(SystemConfiguration systemConfiguration, Camera camera, Guid eventUID);

		void GetVideoFile(SystemConfiguration systemConfiguration, Guid eventUID, Guid cameraUid, string videoPath);

		void AlarmRuleExecute(SystemConfiguration systemConfiguration, string ruleName);

		bool PrepareToTranslation(SystemConfiguration systemConfiguration, Camera camera, out IPEndPoint ipEndPoint, out int vendorId);
	}
}