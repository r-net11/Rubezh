using Common;
using RubezhAPI.Models;
using RviClient.RVIServiceReference;
using RviClient.RVIStreamingServiceReference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;

namespace RviClient
{
	public static class RviClientHelper
	{
		static IntegrationClient CreateIntegrationClient(string url)
		{
			var devices = new List<Device>();
			var binding = new NetTcpBinding(SecurityMode.None);
			binding.OpenTimeout = TimeSpan.FromMinutes(10);
			binding.SendTimeout = TimeSpan.FromMinutes(2);
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
			var endpointAddress = new EndpointAddress(new Uri(url));

			var client = new IntegrationClient(binding, endpointAddress);
			return client;
		}

		static IntegrationVideoStreamingClient CreateIntegrationVideoStreamingClient(RviSettings rviSettings)
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
			var ip = rviSettings.Ip;
			var port = rviSettings.Port;
			var login = rviSettings.Login;
			var password = rviSettings.Password;
			var endpointAddress = new EndpointAddress(new Uri("net.tcp://" + ip + ":" + port + "/IntegrationVideoStreaming"));

			var client = new IntegrationVideoStreamingClient(binding, endpointAddress);
			return client;
		}
		[DebuggerStepThrough]
		public static bool IsConnected(string url, string login, string password)
		{
			try
			{
				using (IntegrationClient client = CreateIntegrationClient(url))
				{
					var sessionUID = Guid.NewGuid();

					var sessionInitialiazationIn = new SessionInitialiazationIn();
					sessionInitialiazationIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					sessionInitialiazationIn.Login = login;
					sessionInitialiazationIn.Password = password;
					var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);

					var sessionCloseIn = new SessionCloseIn();
					sessionCloseIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					var sessionCloseOut = client.SessionClose(sessionCloseIn);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public static List<RviServer> GetServers(string url, string login, string password, List<Camera> existingCameras)
		{
			Server[] servers = null;
			List<RviServer> rviServers = new List<RviServer>();

			using (IntegrationClient client = CreateIntegrationClient(url))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = login;
				sessionInitialiazationIn.Password = password;
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);
				//var errorMessage = sessionInitialiazationOut.Header.HeaderResponseMessage.Information;

				var serverListIn = new ServerListIn();
				serverListIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var serverListOut = client.GetServerList(serverListIn);
				if (serverListOut != null)
				{
					servers = serverListOut.ServerList;
				}

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
				foreach (var server in servers)
				{
					var rviServer = new RviServer { Ip = server.IP, Port = server.Port, Protocol = server.Protocol, Url = server.Url };
					rviServer.RviDevices = GetRviDevices(server.Url, login, password, existingCameras);
					rviServers.Add(rviServer);
				}
			}
			return rviServers;
		}
		static List<RviDevice> GetRviDevices(string url, string login, string password, List<Camera> existingCameras)
		{
			var devices = GetDevices(url, login, password);
			var rviDevices = new List<RviDevice>();
			foreach (var device in devices)
			{
				var rviDevice = new RviDevice { Uid = device.Guid, Ip = device.Ip, Name = device.Name, Status = GetRviStatus(device.Status) };
				rviDevices.Add(rviDevice);
				foreach (var channel in device.Channels)
				{
					var rviChannel = new RviChannel { Name = channel.Name, Number = channel.Number, Vendor = channel.Vendor };
					rviDevice.RviChannels.Add(rviChannel);
					foreach (var stream in channel.Streams)
					{
						var existingCamera = existingCameras.FirstOrDefault(x => x.IsAddedInConfiguration && x.RviDeviceUID == device.Guid && x.RviChannelNo == channel.Number && x.StreamNo == stream.Number);
						if (existingCamera == null)
						{
							var camera = new Camera
							{
								StreamNo = stream.Number,
								Ip = device.Ip,
								RviServerUrl = url,
								RviDeviceName = device.Name,
								RviDeviceUID = device.Guid,
								RviChannelNo = channel.Number,
								RviChannelName = channel.Name,
								CountPresets = channel.CountPresets,
								CountTemplateBypass = channel.CountTemplateBypass,
								CountTemplatesAutoscan = channel.CountTemplatesAutoscan
							};
							rviChannel.Cameras.Add(camera);
						}
						else
						{
							rviChannel.Cameras.Add(existingCamera);
						}
					}
				}
			}
			return rviDevices;
		}
		public static List<RviDevice> GetRviDevicesWithoutChannels(string url, string login, string password)
		{
			var devices = GetDevices(url, login, password);
			var rviDevices = new List<RviDevice>();
			foreach (var device in devices)
			{
				var rviDevice = new RviDevice { Uid = device.Guid, Ip = device.Ip, Name = device.Name, Status = GetRviStatus(device.Status) };
				rviDevices.Add(rviDevice);
			}
			return rviDevices;
		}
		[DebuggerStepThrough]
		static List<Device> GetDevices(string url, string login, string password)
		{
			var devices = new List<Device>();
			try
			{
				using (IntegrationClient client = CreateIntegrationClient(url))
				{
					var sessionUID = Guid.NewGuid();

					var sessionInitialiazationIn = new SessionInitialiazationIn();
					sessionInitialiazationIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					sessionInitialiazationIn.Login = login;
					sessionInitialiazationIn.Password = password;
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
						devices = perimeterOut.Devices.ToList();
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
			catch (Exception)
			{ }
			return devices;
		}

		public static void GetSnapshot(RviSettings rviSettings, Camera camera)
		{
			using (IntegrationClient client = CreateIntegrationClient(rviSettings.Url))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = rviSettings.Login;
				sessionInitialiazationIn.Password = rviSettings.Password;
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

		public static void SetPtzPreset(RviSettings rviSettings, Camera camera, int number)
		{
			using (IntegrationClient client = CreateIntegrationClient(rviSettings.Url))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = rviSettings.Login;
				sessionInitialiazationIn.Password = rviSettings.Password;
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

		public static void VideoRecordStart(RviSettings rviSettings, Camera camera, Guid eventUID, int timeout)
		{
			try
			{
				using (IntegrationClient client = CreateIntegrationClient(rviSettings.Url))
				{
					var sessionUID = Guid.NewGuid();

					var sessionInitialiazationIn = new SessionInitialiazationIn();
					sessionInitialiazationIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					sessionInitialiazationIn.Login = rviSettings.Login;
					sessionInitialiazationIn.Password = rviSettings.Password;
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

					var sessionCloseIn = new SessionCloseIn();
					sessionCloseIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					var sessionCloseOut = client.SessionClose(sessionCloseIn);
				}
			}
			catch (CommunicationException e)
			{
				Logger.Error(e, "RViClientHelper.VideoRecordStart");
			}
		}

		public static void VideoRecordStop(RviSettings rviSettings, Camera camera, Guid eventUID)
		{
			try
			{
				using (IntegrationClient client = CreateIntegrationClient(rviSettings.Url))
				{
					var sessionUID = Guid.NewGuid();

					var sessionInitialiazationIn = new SessionInitialiazationIn();
					sessionInitialiazationIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					sessionInitialiazationIn.Login = rviSettings.Login;
					sessionInitialiazationIn.Password = rviSettings.Password;
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

					var sessionCloseIn = new SessionCloseIn();
					sessionCloseIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					var sessionCloseOut = client.SessionClose(sessionCloseIn);
				}
			}
			catch (CommunicationException e)
			{
				Logger.Error(e, "RViClientHelper.VideoRecordStop");
			}
		}

		public static bool GetVideoFile(RviSettings rviSettings, Guid eventUID, Camera camera, string videoPath, out string errorInformation)
		{
			try
			{
				using (IntegrationClient client = CreateIntegrationClient(rviSettings.Url))
				{
					var sessionUID = Guid.NewGuid();

					var sessionInitialiazationIn = new SessionInitialiazationIn();
					sessionInitialiazationIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					sessionInitialiazationIn.Login = rviSettings.Login;
					sessionInitialiazationIn.Password = rviSettings.Password;
					var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);

					using (IntegrationVideoStreamingClient streamingClient = CreateIntegrationVideoStreamingClient(rviSettings))
					{
						System.IO.Stream stream = null;
						var requestUID = new Guid();
						var result = streamingClient.GetVideoFile(camera.RviChannelNo, camera.RviDeviceUID, eventUID, ref requestUID, ref sessionUID, out errorInformation, out stream);
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
					if (errorInformation != "Запись существует")
						return false;
					return true;
				}
			}
			catch (CommunicationException e)
			{
				Logger.Error(e, "RViClientHelper.GetVideoFile");
				errorInformation = "Видео не получено. Проверьте запущен ли RVi Оператор, правильно ли указаны логин и пароль.";
				return false;
			}
		}
		public static bool PrepareToTranslation(RviSettings rviSettings, Camera camera, out IPEndPoint ipEndPoint, out int vendorId)
		{
			ipEndPoint = null;
			vendorId = -1;

			var devices = GetDevices(rviSettings.Url, rviSettings.Login, rviSettings.Password);
			var device = devices.FirstOrDefault(d => d.Guid == camera.RviDeviceUID);

			if (device == null)
				return false;

			var channel = device.Channels.FirstOrDefault(ch => ch.Number == camera.RviChannelNo);

			if (channel == null)
				return false;

			vendorId = channel.Vendor;

			using (IntegrationClient client = CreateIntegrationClient(rviSettings.Url))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = rviSettings.Login;
				sessionInitialiazationIn.Password = rviSettings.Password;
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

		public static void AlarmRuleExecute(RviSettings rviSettings, string ruleName)
		{
			using (IntegrationClient client = CreateIntegrationClient(rviSettings.Url))
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = rviSettings.Login;
				sessionInitialiazationIn.Password = rviSettings.Password;
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

		public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
		{
			var buffer = new byte[8 * 1024];
			int length;
			while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, length);
			}
			output.Close();
		}
		static RviStatus GetRviStatus(DeviceStatus deviceStatus)
		{
			switch (deviceStatus)
			{
				case DeviceStatus.Connected:
					return RviStatus.Connected;
				case DeviceStatus.Connecting:
					return RviStatus.Connecting;
				case DeviceStatus.Error:
					return RviStatus.Error;
				default:
					return RviStatus.Unknown;
			}
		}
	}
}