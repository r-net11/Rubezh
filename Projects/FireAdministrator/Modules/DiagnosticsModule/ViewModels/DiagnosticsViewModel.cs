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
using RviClient;
using System.Threading;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			AddJournalCommand = new RelayCommand(OnAddJournal);
			AddManyJournalCommand = new RelayCommand(OnAddManyJournal);
			SaveCommand = new RelayCommand(OnSave);
			LoadCommand = new RelayCommand(OnLoad);
			StartVideoCommand = new RelayCommand(OnStartVideo);
			GetVideoCommand = new RelayCommand(OnGetVideo);
		}

		Guid EventUID;

		public RelayCommand StartVideoCommand { get; private set; }
		void OnStartVideo()
		{
			EventUID = Guid.NewGuid();
			var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault();
			RviClientHelper.VideoRecordStart(FiresecManager.SystemConfiguration, camera, EventUID, 5);
		}

		public RelayCommand GetVideoCommand { get; private set; }
		void OnGetVideo()
		{
			var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault();
			if (camera == null)
				return;
			const string fileName = @"C:\Videio.avi";
			RviClientHelper.GetVideoFile(FiresecManager.SystemConfiguration, EventUID, camera.UID, fileName);
			var videoViewModel = new VideoViewModel(fileName);
			DialogService.ShowModalWindow(videoViewModel);
		}

		public string SavedVideoSource
		{
			get { return "C:/Video.avi"; }
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

		public void StopThreads()
		{

		}
	}
}