using System;
using System.Windows.Media;
using DiagnosticsModule.Models;
using FiresecAPI.Journal;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient.RVIServiceReference;
using System.ServiceModel;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;

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
			SessionInitialiazationCommand = new RelayCommand(OnSessionInitialiazation);
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

		public RelayCommand SessionInitialiazationCommand { get; private set; }
		void OnSessionInitialiazation()
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

			var endpointAddress = new EndpointAddress(new Uri("net.tcp://172.16.5.7:8000/Integration"));

			using (IntegrationClient client = new IntegrationClient(binding, endpointAddress))
			//using (IntegrationClient client = new IntegrationClient())
			{
				var sessionUID = Guid.NewGuid();

				var sessionInitialiazationIn = new SessionInitialiazationIn();
				sessionInitialiazationIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				sessionInitialiazationIn.Login = "itest";
				sessionInitialiazationIn.Password = "itest";
				var sessionInitialiazationOut = client.SessionInitialiazation(sessionInitialiazationIn);

				var perimeterIn = new PerimeterIn();
				perimeterIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var perimeterOut = client.GetPerimeter(perimeterIn);
				var devices = perimeterOut.Devices;

				var sessionKeepAliveIn = new SessionKeepAliveIn();
				sessionKeepAliveIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				var sessionKeepAliveOut = client.SessionKeepAlive(sessionKeepAliveIn);

				var videoRecordStartIn = new VideoRecordStartIn();
				videoRecordStartIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				videoRecordStartIn.DeviceGuid = Guid.Empty;
				videoRecordStartIn.ChannelNumber = 0;
				videoRecordStartIn.EventGuid = Guid.Empty;
				videoRecordStartIn.TimeOut = 60;
				var videoRecordStartOut = client.VideoRecordStart(videoRecordStartIn);
				var startTime = videoRecordStartOut.StarTime;

				var videoRecordStopIn = new VideoRecordStopIn();
				videoRecordStopIn.Header = new HeaderRequest()
				{
					Request = Guid.NewGuid(),
					Session = sessionUID
				};
				videoRecordStopIn.DeviceGuid = Guid.Empty;
				videoRecordStopIn.ChannelNumber = 0;
				videoRecordStopIn.EventGuid = Guid.Empty;
				var videoRecordStopOut = client.VideoRecordStop(videoRecordStopIn);

				var sessionCloseIn = new SessionCloseIn();
				sessionKeepAliveIn.Header = new HeaderRequest()
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