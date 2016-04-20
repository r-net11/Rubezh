//using System;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Runtime.Serialization;
//using FS2Api;
//using Infrastructure.Common.Windows;
//using Infrastructure.Common.Windows.Windows.ViewModels;
//using Microsoft.Win32;

//namespace DevicesModule.ViewModels
//{
//	public class FS2DeviceJournalViewModel : DialogViewModel
//	{
//		FS2JournalItemsCollection FS2JournalItemsCollection;

//		public FS2DeviceJournalViewModel(FS2JournalItemsCollection fs2JournalItemsCollection)
//		{
//			Title = "Журнал событий";
//			SaveToFileCommand = new RelayCommand(OnSaveToFile);
//			FS2JournalItemsCollection = fs2JournalItemsCollection;
//			CreationDateTime = fs2JournalItemsCollection.CreationDateTime;

//			FireJournalItems = new ObservableCollection<FS2JournalItem>();
//			foreach (var journalItem in fs2JournalItemsCollection.FireJournalItems)
//			{
//				FireJournalItems.Add(journalItem);
//			}

//			SecurityJournalItems = new ObservableCollection<FS2JournalItem>();
//			foreach (var journalItem in fs2JournalItemsCollection.SecurityJournalItems)
//			{
//				SecurityJournalItems.Add(journalItem);
//			}
//		}

//		public DateTime CreationDateTime { get; set; }
//		public ObservableCollection<FS2JournalItem> FireJournalItems { get; set; }
//		public ObservableCollection<FS2JournalItem> SecurityJournalItems { get; set; }

//		public bool HasSecurityJournalItems
//		{
//			get { return SecurityJournalItems.Count > 0; }
//		}

//		public RelayCommand SaveToFileCommand { get; private set; }
//		void OnSaveToFile()
//		{
//			var saveDialog = new SaveFileDialog()
//			{
//				Filter = "Журнал событий Firesec|*.fscj",
//				DefaultExt = "Журнал событий Firesec|*.fscj"
//			};
//			if (saveDialog.ShowDialog().Value)
//			{
//				WaitHelper.Execute(() =>
//				{
//					if (File.Exists(saveDialog.FileName))
//						File.Delete(saveDialog.FileName);

//					var xmlSerializer = new XmlSerializer(typeof(FS2JournalItemsCollection));
//					using (var fileStream = new FileStream(saveDialog.FileName, FileMode.CreateNew))
//					{
//						xmlSerializer.Serialize(fileStream, FS2JournalItemsCollection);
//					}
//				});
//			}
//		}
//	}
//}