using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Common;
using StrazhAPI.Models;
using RviCommonClient;
using RviIntegratorClient.RviIntegrationServiceReference;
using RviIntegratorClient.RviIntegrationVideoStreamingServiceReference;

namespace RviIntegratorClient
{
    public class RviIntegratorIntegrationClient : IRviClient
	{
		#region <Реализация интерфейса IRviClient>

		public List<IRviDevice> GetDevices(SystemConfiguration systemConfiguration)
		{
			var devices = new List<IRviDevice>();

			using (var client = CreateIntegrationClient(systemConfiguration))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = systemConfiguration.RviSettings.Login;
				sessionInitialiazationIn.Password = systemConfiguration.RviSettings.Password;
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);
				//var errorMessage = sessionInitialiazationOut.Header.HeaderResponseMessage.Information;

				var perimeterIn = new PerimeterIn();
				perimeterIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var perimeterOut = client.GetPerimeter(perimeterIn);
				
				Logger.Info("Вызов RviИнтегратор.GetDevices");
				if (perimeterOut.Header.HeaderResponseMessage.Code != 0)
					Logger.Error(string.Format("Ошибка при вызове RviИнтегратор.GetDevices: {0}", perimeterOut.Header.HeaderResponseMessage.Information));

				if (perimeterOut.Devices != null)
				{
					var perimeterDevices = perimeterOut.Devices.Where(x => !x.IsDeleted).ToList();
					devices.AddRange(perimeterDevices.Select(perimeterDevice => CreateDevice(perimeterDevice)));
				}

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}

			return devices;
		}

		public void GetSnapshot(SystemConfiguration systemConfiguration, Camera camera)
		{
			using (var client = CreateIntegrationClient(systemConfiguration))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = systemConfiguration.RviSettings.Login;
				sessionInitialiazationIn.Password = systemConfiguration.RviSettings.Password;
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);

				var snapshotDoIn = new SnapshotDoIn();
				snapshotDoIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};

				var snapshotUID = new Guid();
				snapshotDoIn.DeviceGuid = camera.RviDeviceUID;
				snapshotDoIn.ChannelNumber = camera.RviChannelNo;
				snapshotDoIn.EventGuid = snapshotUID;
				var snapshotDoOut = client.SnapshotDo(new SnapshotDoIn());

				var snapshotImageIn = new SnapshotImageIn();
				snapshotImageIn.DeviceGuid = camera.RviDeviceUID;
				snapshotImageIn.ChannelNumber = camera.RviChannelNo;
				snapshotImageIn.EventGuid = snapshotUID;
				snapshotImageIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};

				Logger.Info("Вызов RviИнтегратор.GetSnapshot");
				var snapshotImageOut = client.GetSnapshotImage(snapshotImageIn);
				if (snapshotImageOut.Header.HeaderResponseMessage.Code != 0)
					Logger.Error(string.Format("Ошибка при вызове RviИнтегратор.GetSnapshot: {0}", snapshotImageOut.Header.HeaderResponseMessage.Information));

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public void SetPtzPreset(SystemConfiguration systemConfiguration, Camera camera, int number)
		{
			using (var client = CreateIntegrationClient(systemConfiguration))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = systemConfiguration.RviSettings.Login;
				sessionInitialiazationIn.Password = systemConfiguration.RviSettings.Password;
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);

				var ptzPresetIn = new PtzPresetIn();
				ptzPresetIn.DeviceGuid = camera.RviDeviceUID;
				ptzPresetIn.ChannelNumber = camera.RviChannelNo;
				ptzPresetIn.Number = number;
				ptzPresetIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};

				Logger.Info("Вызов RviИнтегратор.SetPtzPreset");
				var ptzPresetOut = client.SetPtzPreset(ptzPresetIn);
				if (ptzPresetOut.Header.HeaderResponseMessage.Code != 0)
					Logger.Error(string.Format("Ошибка при вызове RviИнтегратор.SetPtzPreset: {0}", ptzPresetOut.Header.HeaderResponseMessage.Information));

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public void VideoRecordStart(SystemConfiguration systemConfiguration, Camera camera, Guid eventUID, int timeout)
		{
			using (var client = CreateIntegrationClient(systemConfiguration))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = systemConfiguration.RviSettings.Login;
				sessionInitialiazationIn.Password = systemConfiguration.RviSettings.Password;
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);

				var videoRecordStartIn = new VideoRecordStartIn();
				videoRecordStartIn.DeviceGuid = camera.RviDeviceUID;
				videoRecordStartIn.ChannelNumber = camera.RviChannelNo;
				videoRecordStartIn.EventGuid = eventUID;
				videoRecordStartIn.TimeOut = timeout;
				videoRecordStartIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};

				Logger.Info("Вызов RviИнтегратор.VideoRecordStart");
				var videoRecordStartOut = client.VideoRecordStart(videoRecordStartIn);
				if (videoRecordStartOut.Header.HeaderResponseMessage.Code != 0)
					Logger.Error(string.Format("Ошибка при вызове RviИнтегратор.VideoRecordStart: {0}", videoRecordStartOut.Header.HeaderResponseMessage.Information));

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public void VideoRecordStop(SystemConfiguration systemConfiguration, Camera camera, Guid eventUID)
		{
			using (var client = CreateIntegrationClient(systemConfiguration))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = systemConfiguration.RviSettings.Login;
				sessionInitialiazationIn.Password = systemConfiguration.RviSettings.Password;
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);

				var videoRecordStopIn = new VideoRecordStopIn();
				videoRecordStopIn.DeviceGuid = camera.RviDeviceUID;
				videoRecordStopIn.ChannelNumber = camera.RviChannelNo;
				videoRecordStopIn.EventGuid = eventUID;
				videoRecordStopIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};

				Logger.Info("Вызов RviИнтегратор.VideoRecordStop");
				var videoRecordStopOut = client.VideoRecordStop(videoRecordStopIn);
				if (videoRecordStopOut.Header.HeaderResponseMessage.Code != 0)
					Logger.Error(string.Format("Ошибка при вызове RviИнтегратор.VideoRecordStop: {0}", videoRecordStopOut.Header.HeaderResponseMessage.Information));

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public void GetVideoFile(SystemConfiguration systemConfiguration, Guid eventUID, Guid cameraUid, string videoPath)
		{
			var camera = systemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			using (var client = CreateIntegrationClient(systemConfiguration))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = systemConfiguration.RviSettings.Login;
				sessionInitialiazationIn.Password = systemConfiguration.RviSettings.Password;
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);

				using (var streaminClient = CreateIntegrationVideoStreamingClient(systemConfiguration))
				{
					string errorInformation;
					System.IO.Stream stream = null;
					var requestUID = new Guid();

					Logger.Info("Вызов RviИнтегратор.GetVideoFile");
					var result = streaminClient.GetVideoFile(camera.RviChannelNo, camera.RviDeviceUID, eventUID, ref requestUID, ref sessionUID, out errorInformation, out stream);
					if (result != 0)
						Logger.Error(string.Format("Ошибка при вызове RviИнтегратор.GetVideoFile: {0}", errorInformation));

					var videoFileStream = File.Create(videoPath);
					CopyStream(stream, videoFileStream);
				}

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public void AlarmRuleExecute(SystemConfiguration systemConfiguration, string ruleName)
		{
			using (var client = CreateIntegrationClient(systemConfiguration))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = systemConfiguration.RviSettings.Login;
				sessionInitialiazationIn.Password = systemConfiguration.RviSettings.Password;
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);

				var alarmRulesIn = new AlarmRulesIn();
				alarmRulesIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var alarmRulesOut = client.GetAlarmRules(alarmRulesIn);
				if (alarmRulesOut != null && alarmRulesOut.AlarmRules != null)
				{
					var alarmRule = alarmRulesOut.AlarmRules.FirstOrDefault(x => x.Name == ruleName);
					if (alarmRule != null)
					{
						var alarmRuleExecuteIn = new AlarmRuleExecuteIn();
						alarmRuleExecuteIn.Header = new HeaderRequest()
						{
							Request = Guid.NewGuid(),
							Session = sessionUID
						};
						alarmRuleExecuteIn.AlarmRuleGuid = alarmRule.Guid;
						alarmRuleExecuteIn.ExternalEventGuid = Guid.NewGuid();

						Logger.Info("Вызов RviИнтегратор.AlarmRuleExecute");
						var alarmRuleExecuteOut = client.AlarmRuleExecute(alarmRuleExecuteIn);
						if (alarmRuleExecuteOut.Header.HeaderResponseMessage.Code != 0)
							Logger.Error(string.Format("Ошибка при вызове RviИнтегратор.AlarmRuleExecute: {0}", alarmRuleExecuteOut.Header.HeaderResponseMessage.Information));
					}
				}

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public bool PrepareToTranslation(SystemConfiguration systemConfiguration, Camera camera, out System.Net.IPEndPoint ipEndPoint, out int vendorId)
		{
			ipEndPoint = null;
			vendorId = -1;

			var devices = GetDevices(systemConfiguration);
			var device = devices.FirstOrDefault(d => d.Guid == camera.RviDeviceUID);

			if (device == null)
				return false;

			var channel = device.Channels.FirstOrDefault(ch => ch.Number == camera.RviChannelNo);

			if (channel == null)
				return false;

			vendorId = channel.Vendor;

			using (var client = CreateIntegrationClient(systemConfiguration))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = systemConfiguration.RviSettings.Login;
				sessionInitialiazationIn.Password = systemConfiguration.RviSettings.Password;
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);
				//var errorMessage = sessionInitialiazationOut.Header.HeaderResponseMessage.Information;

				Logger.Info("Вызов RviИнтегратор.VideoStreamingStart");
				var response = client.VideoStreamingStart(new ChannelStreamingStartIn()
				{
					Header = new HeaderRequest() { Request = new Guid(), Session = sessionUID },
					DeviceGuid = device.Guid,
					ChannelNumber = channel.Number,
					StreamNumber = camera.StreamNo
				});
				if (response.Header.HeaderResponseMessage.Code != 0)
					Logger.Error(string.Format("Ошибка при вызове RviИнтегратор.VideoStreamingStart: {0}", response.Header.HeaderResponseMessage.Information));

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);

				if (response.EndPointPort == 0)
				{
					return false;
				}
				ipEndPoint = new IPEndPoint(IPAddress.Parse(response.EndPointAdress), response.EndPointPort);
			}

			return true;

		}

		#endregion </Реализация интерфейса IRviClient>

		private static IntegrationClient CreateIntegrationClient(SystemConfiguration systemConfiguration)
		{
			var devices = new List<Device>();
			var binding = new NetTcpBinding(SecurityMode.None);
			binding.OpenTimeout = TimeSpan.FromMinutes(10);
			binding.SendTimeout = TimeSpan.FromMinutes(10);
			binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
			binding.MaxReceivedMessageSize = Int32.MaxValue;
			binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			binding.Security.Mode = SecurityMode.None;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
			var ip = systemConfiguration.RviSettings.Ip;
			var port = systemConfiguration.RviSettings.Port;
			var login = systemConfiguration.RviSettings.Login;
			var password = systemConfiguration.RviSettings.Password;
			var endpointAddress = new EndpointAddress(new Uri("net.tcp://" + ip + ":" + port + "/Integration"));

			var client = new IntegrationClient(binding, endpointAddress);
			return client;
		}

		private static IntegrationVideoStreamingClient CreateIntegrationVideoStreamingClient(SystemConfiguration systemConfiguration)
		{
			var binding = new NetTcpBinding(SecurityMode.None);
			binding.TransferMode = TransferMode.Streamed;
			binding.OpenTimeout = TimeSpan.FromMinutes(10);
			binding.SendTimeout = TimeSpan.FromMinutes(10);
			binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
			binding.MaxReceivedMessageSize = Int32.MaxValue;
			binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			binding.Security.Mode = SecurityMode.None;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
			var ip = systemConfiguration.RviSettings.Ip;
			var port = systemConfiguration.RviSettings.Port;
			var login = systemConfiguration.RviSettings.Login;
			var password = systemConfiguration.RviSettings.Password;
			var endpointAddress = new EndpointAddress(new Uri("net.tcp://" + ip + ":" + port + "/IntegrationVideoStreaming"));

			var client = new IntegrationVideoStreamingClient(binding, endpointAddress);
			return client;
		}

		private static void CopyStream(System.IO.Stream input, System.IO.Stream output)
		{
			var buffer = new byte[8 * 1024];
			int length;
			while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, length);
			}
			output.Close();
		}

		private static IRviStream CreateStream(RviIntegratorClient.RviIntegrationServiceReference.Stream stream)
		{
			return new RviStream
			{
				Number = stream.Number,
				Rtsp = stream.Rtsp
			};
		}

		private static IRviChannel CreateChannel(RviIntegratorClient.RviIntegrationServiceReference.Channel channel)
		{
			return new RviChannel
			{
				Number = channel.Number,
				Name = channel.Name,
				CountPresets = channel.CountPresets,
				CountTemplateBypass = channel.CountTemplateBypass,
				CountTemplatesAutoscan = channel.CountTemplatesAutoscan,
				Vendor = channel.Vendor,
				Streams = channel.Streams.Select(stream => CreateStream(stream)).ToArray()
			};
		}

		private static IRviDevice CreateDevice(RviIntegratorClient.RviIntegrationServiceReference.Device device)
		{
			return new RviDevice
			{
				Guid = device.Guid,
				Ip = device.Ip,
				Name = device.Name,
				Channels = device.Channels.Select(channel => CreateChannel(channel)).ToArray()
			};
		}
	}
}
