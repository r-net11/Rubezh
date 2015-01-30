using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RviClient.RVIServiceReference;
using System.ServiceModel;
using FiresecAPI.Models;
using RviClient.RVIStreamingServiceReference;
using System.IO;

namespace RviClient
{
	public static class RviClientHelper
	{
		static IntegrationClient CreateIntegrationClient(SystemConfiguration systemConfiguration)
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

		static IntegrationVideoStreamingClient CreateIntegrationVideoStreamingClient(SystemConfiguration systemConfiguration)
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

		public static List<Device> GetDevices(SystemConfiguration systemConfiguration)
		{
			var devices = new List<Device>();
			
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
				devices = perimeterOut.Devices.ToList();

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

		public static void GetSnapshot(SystemConfiguration systemConfiguration, Camera camera)
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
				//snapshotImageOut.

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public static void VideoRecordStart(SystemConfiguration systemConfiguration, Camera camera, Guid eventUID, int timeout)
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

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public static void VideoRecordStop(SystemConfiguration systemConfiguration, Camera camera, Guid eventUID)
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

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public static string GetVideoFile(SystemConfiguration systemConfiguration, Camera camera, Guid eventUID)
		{
			string fileName = @"D:/Video.avi";
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

					var videoFileStream = File.Create(fileName);
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
			return fileName;
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
	}
}