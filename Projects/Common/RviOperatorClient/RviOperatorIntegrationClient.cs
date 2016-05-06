using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;
using StrazhAPI.Models;
using RviCommonClient;
using RviOperatorClient.RviIntegrationServiceReference;
using RviOperatorClient.RviIntegrationVideoStreamingServiceReference;

namespace RviOperatorClient
{
	public class RviOperatorIntegrationClient : IRviClient
	{
		#region <Реализация интерфейса IRviClient>

		public List<IRviDevice> GetDevices(SystemConfiguration systemConfiguration)
		{
			var devices = new List<IRviDevice>();

			using (IntegrationClient client = CreateIntegrationClient(systemConfiguration))
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
			using (IntegrationClient client = CreateIntegrationClient(systemConfiguration))
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
				var snapshotImageOut = client.GetSnapshotImage(snapshotImageIn);

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
			using (IntegrationClient client = CreateIntegrationClient(systemConfiguration))
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
				var ptzPresetOut = client.SetPtzPreset(ptzPresetIn);

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
			using (IntegrationClient client = CreateIntegrationClient(systemConfiguration))
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
				var videoRecordStartOut = client.VideoRecordStart(videoRecordStartIn);
#if DEBUG
				Logger.Info(String.Format("Вызов RviОператор.VideoRecordStart(new VideoRecordStartIn(DeviceGuid='{0}', ChannelNumber={1}, EventGuid='{2}', TimeOut={3}, new HeaderRequest(Request='{4}', Session='{5}'))", videoRecordStartIn.DeviceGuid, videoRecordStartIn.ChannelNumber, videoRecordStartIn.EventGuid, videoRecordStartIn.TimeOut, videoRecordStartIn.Header.Request, videoRecordStartIn.Header.Session));
#endif

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
			using (IntegrationClient client = CreateIntegrationClient(systemConfiguration))
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
				var videoRecordStopOut = client.VideoRecordStop(videoRecordStopIn);
#if DEBUG
				Logger.Info(String.Format("Вызов RviОператор.VideoRecordStop(new VideoRecordStopIn(DeviceGuid='{0}', ChannelNumber={1}, EventGuid='{2}', new HeaderRequest(Request='{3}', Session='{4}'))", videoRecordStopIn.DeviceGuid, videoRecordStopIn.ChannelNumber, videoRecordStopIn.EventGuid, videoRecordStopIn.Header.Request, videoRecordStopIn.Header.Session));
#endif

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
			using (IntegrationClient client = CreateIntegrationClient(systemConfiguration))
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

				using (IntegrationVideoStreamingClient streaminClient = CreateIntegrationVideoStreamingClient(systemConfiguration))
				{
					string errorInformation;
					System.IO.Stream stream = null;
					var requestUID = new Guid();
					var result = streaminClient.GetVideoFile(camera.RviChannelNo, camera.RviDeviceUID, eventUID, ref requestUID, ref sessionUID, out errorInformation, out stream);
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
			using (IntegrationClient client = CreateIntegrationClient(systemConfiguration))
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
						var alarmRuleExecuteOut = client.AlarmRuleExecute(alarmRuleExecuteIn);
#if DEBUG
						Logger.Info(String.Format("Вызов RviОператор.AlarmRuleExecute(new AlarmRuleExecuteIn(Request='{0}', Session='{1}'))", alarmRuleExecuteIn.Header.Request, alarmRuleExecuteIn.Header.Session));
#endif
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

			using (IntegrationClient client = CreateIntegrationClient(systemConfiguration))
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

				var response = client.VideoStreamingStart(new ChannelStreamingStartIn()
				{
					Header = new HeaderRequest() { Request = new Guid(), Session = sessionUID },
					DeviceGuid = device.Guid,
					ChannelNumber = channel.Number,
					StreamNumber = camera.StreamNo
				});

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

		private static IRviStream CreateStream(RviOperatorClient.RviIntegrationServiceReference.Stream stream)
		{
			return new RviStream
			{
				Number = stream.Number,
				Rtsp = stream.Rtsp
			};
		}

		private static IRviChannel CreateChannel(RviOperatorClient.RviIntegrationServiceReference.Channel channel)
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

		private static IRviDevice CreateDevice(RviOperatorClient.RviIntegrationServiceReference.Device device)
		{
			return new RviDevice
			{
				Guid = device.Guid,
				Ip = device.Ip,
				Name = device.Name,
				Channels = device.Channels.Select(channel =>CreateChannel(channel)).ToArray()
			};
		}
	}
}
