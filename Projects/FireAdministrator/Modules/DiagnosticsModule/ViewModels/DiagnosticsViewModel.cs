using System;
using System.Linq;
using System.Windows.Media;
using DiagnosticsModule.Models;
using FiresecAPI.Journal;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.ServiceModel;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;
using Infrastructure.Common.Windows;
using RviClient.RVIServiceReference;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		int Count = 0;

		public DiagnosticsViewModel()
		{
			AddJournalCommand = new RelayCommand(OnAddJournal);
			AddManyJournalCommand = new RelayCommand(OnAddManyJournal);
			SaveCommand = new RelayCommand(OnSave);
			LoadCommand = new RelayCommand(OnLoad);
			RviTestCommand = new RelayCommand(OnRviTest);
			StartVlc = new RelayCommand(OnStartVlc);
		}

		private VlcControl _vlcControl;
		public ImageSource Image
		{
			get
			{
				return _vlcControl.VideoSource;
			}
		}

		private void VlcControlOnPositionChanged(VlcControl sender, VlcEventArgs<float> vlcEventArgs)
		{
			OnPropertyChanged("Image");
		}

		public RelayCommand StartVlc { get; private set; }
		void OnStartVlc()
		{
			if (!VlcContext.IsInitialized)
			{
				//Set libvlc.dll and libvlccore.dll directory path
				VlcContext.LibVlcDllsPath = CommonStrings.LIBVLC_DLLS_PATH_DEFAULT_VALUE_AMD64;
				//Set the vlc plugins directory path
				VlcContext.LibVlcPluginsPath = CommonStrings.PLUGINS_PATH_DEFAULT_VALUE_AMD64;

				//Set the startup options
				VlcContext.StartupOptions.IgnoreConfig = true;
				VlcContext.StartupOptions.LogOptions.LogInFile = false;
				VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = true;
				VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;

				//Initialize the VlcContext
				VlcContext.Initialize();
			}
			_vlcControl = new VlcControl { Media = new LocationMedia("rtsp://admin:admin@172.16.2.23:554/cam/realmonitor?channel=1&subtype=0") };
			_vlcControl.PositionChanged += VlcControlOnPositionChanged;
			_vlcControl.Play();
		}

		public RelayCommand AddJournalCommand { get; private set; }
		void OnAddJournal()
		{
			var journalItem = new JournalItem();
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = JournalEventNameType.Подтверждение_тревоги;
			FiresecManager.FiresecService.AddJournalItem(journalItem);
		}

		public RelayCommand AddManyJournalCommand { get; private set; }
		void OnAddManyJournal()
		{
			var subsystemTypes = Enum.GetValues(typeof(JournalSubsystemType));
			var nameTypes = Enum.GetValues(typeof(JournalEventNameType));
			var descriptionTypes = Enum.GetValues(typeof(JournalEventDescriptionType));
			var objectTypes = Enum.GetValues(typeof(JournalObjectType));
			var rnd = new Random();
			for (int i = 0; i < 10000; i++)
			{
				var journalItem = new JournalItem();
				journalItem.DeviceDateTime = DateTime.Now;
				journalItem.JournalSubsystemType = (JournalSubsystemType)subsystemTypes.GetValue(rnd.Next(subsystemTypes.Length));
				journalItem.JournalEventNameType = (JournalEventNameType)nameTypes.GetValue(rnd.Next(nameTypes.Length));
				journalItem.JournalEventDescriptionType = (JournalEventDescriptionType)descriptionTypes.GetValue(rnd.Next(descriptionTypes.Length));
				journalItem.JournalObjectType = (JournalObjectType)objectTypes.GetValue(rnd.Next(objectTypes.Length));
				FiresecManager.FiresecService.AddJournalItem(journalItem);
			}

		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			SerializerHelper.Save();
		}

		public RelayCommand LoadCommand { get; private set; }
		void OnLoad()
		{
			SerializerHelper.Load();
		}

		public RelayCommand RviTestCommand { get; private set; }
		void OnRviTest()
		{
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
			var ip = FiresecManager.SystemConfiguration.RviSettings.Ip;
			var port = FiresecManager.SystemConfiguration.RviSettings.Port;
			var login = FiresecManager.SystemConfiguration.RviSettings.Login;
			var password = FiresecManager.SystemConfiguration.RviSettings.Password;
			var endpointAddress = new EndpointAddress(new Uri("net.tcp://" + ip + ":" + port + "/Integration"));

			using (IntegrationClient client = new IntegrationClient(binding, endpointAddress))
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

				var snapshotDoIn = new SnapshotDoIn();
				snapshotDoIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};

				var snapshotUID = new Guid();
				var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault();
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


				var eventUID = new Guid();
				var videoRecordStartIn = new VideoRecordStartIn();
				videoRecordStartIn.DeviceGuid = camera.RviDeviceUID;
				videoRecordStartIn.ChannelNumber = camera.RviChannelNo;
				videoRecordStartIn.EventGuid = eventUID;
				videoRecordStartIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				client.VideoRecordStart(videoRecordStartIn);

				var sessionCloseIn = new SessionCloseIn();
				sessionCloseIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionCloseOut = client.SessionClose(sessionCloseIn);
			}
		}

		public void StopThreads()
		{
			IsThreadStoping = true;
		}
		bool IsThreadStoping = false;

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}
	}
}