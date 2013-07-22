using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FS2Api;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.IO;
using Microsoft.Win32;
using System.Runtime.Serialization;

namespace DevicesModule.ViewModels
{
	public class FS2DeviceJournalViewModel : DialogViewModel
	{
		FS2JournalItemsCollection FS2JournalItemsCollection;

		public FS2DeviceJournalViewModel(FS2JournalItemsCollection fs2JournalItemsCollection)
		{
			Title = "Журнал событий";
			SaveToFileCommand = new RelayCommand(OnSaveToFile);
			FS2JournalItemsCollection = fs2JournalItemsCollection;
			
			FireJournalItems = new ObservableCollection<FS2JournalItem>();
			foreach (var journalItem in fs2JournalItemsCollection.FireJournalItems)
			{
				FireJournalItems.Add(journalItem);
			}

			SecurityJournalItems = new ObservableCollection<FS2JournalItem>();
			foreach (var journalItem in fs2JournalItemsCollection.SecurityJournalItems)
			{
				SecurityJournalItems.Add(journalItem);
			}
		}

		public ObservableCollection<FS2JournalItem> FireJournalItems { get; set; }
		public ObservableCollection<FS2JournalItem> SecurityJournalItems { get; set; }

		public bool HasSecurityJournalItems
		{
			get { return SecurityJournalItems.Count > 0; }
		}

		public RelayCommand SaveToFileCommand { get; private set; }
		void OnSaveToFile()
		{
			var saveDialog = new SaveFileDialog()
			{
				Filter = "Журнал событий Firesec-2|*.fscj",
				DefaultExt = "Журнал событий Firesec-2|*.fscj"
			};
			if (saveDialog.ShowDialog().Value)
			{
				WaitHelper.Execute(() =>
				{
					if (File.Exists(saveDialog.FileName))
						File.Delete(saveDialog.FileName);

					var dataContractSerializer = new DataContractSerializer(typeof(FS2JournalItemsCollection));
					using (var fileStream = new FileStream(saveDialog.FileName, FileMode.CreateNew))
					{
						dataContractSerializer.WriteObject(fileStream, FS2JournalItemsCollection);
					}
				});
			}
		}
	}
}