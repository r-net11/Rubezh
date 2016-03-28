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
		public static List<RviServer> GetServers(string url, string login, string password, List<Camera> existingCameras)
		{
			List<RviServer> rviServers = new List<RviServer>();
			try
			{
				Server[] servers = null;
				bool isNotConnected;
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
					servers = serverListOut.ServerList;

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
						rviServer.RviDevices = GetRviDevices(server.Url, login, password, existingCameras, out isNotConnected);
						rviServer.Status = isNotConnected ? RviStatus.ConnectionLost : RviStatus.Connected;
						rviServers.Add(rviServer);
					}
				}
			}
			catch (CommunicationObjectFaultedException) { }
			return rviServers;
		}
		public static List<RviDevice> GetRviDevices(string url, string login, string password, List<Camera> existingCameras, out bool isNotConnected)
		{
			var devices = GetDevices(url, login, password, out isNotConnected);
			var rviDevices = new List<RviDevice>();
			var newCameras = new List<Camera>();
			foreach (var device in devices)
			{
				var rviDevice = new RviDevice { Uid = device.Guid, Ip = device.Ip, Name = device.Name, Status = ConvertToRviStatus(device.Status) };
				rviDevices.Add(rviDevice);
				foreach (var channel in device.Channels)
				{
					var camera = new Camera
					{
						UID = channel.Guid,
						Name = channel.Name,
						RviServerUrl = url,
						RviDeviceName = device.Name,
						RviDeviceUID = device.Guid,
						Number = channel.Number,
						Vendor = channel.Vendor,
						CountPresets = channel.CountPresets,
						CountTemplateBypass = channel.CountTemplateBypass,
						CountTemplatesAutoscan = channel.CountTemplatesAutoscan,
						Status = ConvertToRviStatus(device.Status),
						IsRecordOnline = channel.IsRecordOnline,
						IsOnGuard = channel.IsOnGuard
					};
					foreach (var stream in channel.Streams)
					{
						var rviStream = new RviStream { Number = stream.Number, RviDeviceUid = device.Guid, RviChannelNumber = channel.Number };
						camera.RviStreams.Add(rviStream);
					}
					camera.SelectedRviStreamNumber = camera.RviStreams.Count > 0 ? camera.RviStreams.First().Number : 0;
					rviDevice.Cameras.Add(camera);
					newCameras.Add(camera);
				}
			}
			if (existingCameras != null)
			{
				foreach (var existingCamera in existingCameras)
				{
					var camera = newCameras.FirstOrDefault(newCamera => newCamera.UID == existingCamera.UID);
					if (camera != null)
					{
						camera.ShowDetailsHeight = existingCamera.ShowDetailsHeight;
						camera.ShowDetailsWidth = existingCamera.ShowDetailsWidth;
						camera.ShowDetailsMarginLeft = existingCamera.ShowDetailsMarginLeft;
						camera.ShowDetailsMarginTop = existingCamera.ShowDetailsMarginTop;
					}
				}
			}
			return rviDevices;
		}
		[DebuggerStepThrough]
		static List<Device> GetDevices(string url, string login, string password, out bool isNotConnected)
		{
			var devices = new List<Device>();
			isNotConnected = false;
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
			{
				isNotConnected = true;
			}
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
				snapshotDoIn.ChannelNumber = camera.Number;
				snapshotDoIn.EventGuid = snapshotUID;
				var snapshotDoOut = client.SnapshotDo(new SnapshotDoIn());

				var snapshotImageIn = new SnapshotImageIn();
				snapshotImageIn.DeviceGuid = camera.RviDeviceUID;
				snapshotImageIn.ChannelNumber = camera.Number;
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
				ptzPresetIn.ChannelNumber = camera.Number;
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
					videoRecordStartIn.ChannelNumber = camera.Number;
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
					videoRecordStopIn.ChannelNumber = camera.Number;
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
		public static void AlarmSetChannel(RviSettings rviSettings, Camera camera)
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

					var alarmSetChannelIn = new AlarmSetChannelIn();
					alarmSetChannelIn.DeviceGuid = camera.RviDeviceUID;
					alarmSetChannelIn.ChannelNumber = camera.Number;
					alarmSetChannelIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					var alarmSetChannelOut = client.AlarmSetChannel(alarmSetChannelIn);

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
				Logger.Error(e, "RViClientHelper.AlarmSetChannel");
			}
		}
		public static void AlarmDisableChannel(RviSettings rviSettings, Camera camera)
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

					var alarmDisableChannelIn = new AlarmDisableChannelIn();
					alarmDisableChannelIn.DeviceGuid = camera.RviDeviceUID;
					alarmDisableChannelIn.ChannelNumber = camera.Number;
					alarmDisableChannelIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					var alarmDisableChannelOut = client.AlarmDisableChannel(alarmDisableChannelIn);

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
				Logger.Error(e, "RViClientHelper.AlarmDisableChannel");
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
						var result = streamingClient.GetVideoFile(camera.Number, camera.RviDeviceUID, eventUID, ref requestUID, ref sessionUID, out errorInformation, out stream);
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
				errorInformation = "Видео не получено. Проверьте запущен ли сервер RVi, правильно ли указаны логин и пароль.";
				return false;
			}
		}
		public static bool PrepareToTranslation(RviSettings rviSettings, RviStream rviStream, out IPEndPoint ipEndPoint, out int vendorId)
		{
			ipEndPoint = null;
			vendorId = -1;
			bool isNotConnected;

			var devices = GetDevices(rviSettings.Url, rviSettings.Login, rviSettings.Password, out isNotConnected);
			var device = devices.FirstOrDefault(d => d.Guid == rviStream.RviDeviceUid);

			if (device == null)
				return false;

			var channel = device.Channels.FirstOrDefault(ch => ch.Number == rviStream.RviChannelNumber);

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
					StreamNumber = rviStream.Number
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
						alarmRuleExecuteIn.AlarmRuleGuid = alarmRule.Guid;
						alarmRuleExecuteIn.ExternalEventGuid = Guid.NewGuid();
						alarmRuleExecuteIn.Header = new HeaderRequest()
						{
							Request = Guid.NewGuid(),
							Session = sessionUID
						};
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
		public static void OpenWindow(RviSettings rviSettings, string name, int x, int y, int monitorNumber, string login, string ip)
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
				var windowListIn = new WindowListIn();
				windowListIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var windowListOut = client.GetWindowList(windowListIn);

				var windowClient = windowListOut.Window.FirstOrDefault(window => window.Name == name);
				if (windowClient != null)
				{
					var openWindowIn = new OpenWindowIn();
					openWindowIn.IP = ip;
					openWindowIn.Login = login;
					openWindowIn.GuidWindow = windowClient.Guid;
					openWindowIn.Monitor = new Monitor() { X = x, Y = y, Number = monitorNumber };
					openWindowIn.Header = new HeaderRequest()
					{
						Request = Guid.NewGuid(),
						Session = sessionUID
					};
					var openWindowOut = client.OpenWindow(openWindowIn);
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
		static RviStatus ConvertToRviStatus(DeviceStatus deviceStatus)
		{
			switch (deviceStatus)
			{
				case DeviceStatus.Connected:
					return RviStatus.Connected;
				case DeviceStatus.Connecting:
					return RviStatus.Connecting;
				case DeviceStatus.Error:
				default:
					return RviStatus.Error;
			}
		}
	}
}