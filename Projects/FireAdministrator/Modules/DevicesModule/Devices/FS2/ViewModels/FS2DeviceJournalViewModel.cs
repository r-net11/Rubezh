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

		public FS2DeviceJournalViewModel(List<FS2JournalItem> journalItems)
		{
			Title = "Журнал событий";
			SaveToFileCommand = new RelayCommand(OnSaveToFile);

			FS2JournalItemsCollection = new FS2JournalItemsCollection()
			{
				FireJournalItems = journalItems
			};
			JournalItems = new ObservableCollection<FS2JournalItem>();
			foreach (var journalItem in journalItems)
			{
				JournalItems.Add(journalItem);
			}
		}

		public ObservableCollection<FS2JournalItem> JournalItems { get; set; }

		public RelayCommand SaveToFileCommand { get; private set; }
		void OnSaveToFile()
		{
			var saveDialog = new SaveFileDialog()
			{
				Filter = "firesec2 journal files|*.fscj",
				DefaultExt = "firesec2 journal files|*.fscj"
			};
			if (saveDialog.ShowDialog().Value)
			{
				WaitHelper.Execute(() =>
				{
					var dataContractSerializer = new DataContractSerializer(typeof(FS2JournalItemsCollection));
					using (var fileStream = new FileStream(saveDialog.FileName, FileMode.CreateNew, FileAccess.Write))
					{
						dataContractSerializer.WriteObject(fileStream, FS2JournalItemsCollection);
					}
				});
			}
		}
	}
}