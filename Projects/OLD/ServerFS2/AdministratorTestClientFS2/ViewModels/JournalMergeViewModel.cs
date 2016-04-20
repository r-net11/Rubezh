using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using FS2Api;

namespace AdministratorTestClientFS2.ViewModels
{
	public class JournalMergeViewModel : DialogViewModel
	{
		public JournalMergeViewModel(FS2JournalItemsCollection fs2JournalItemsCollection)
		{
			Title = "Сравнение журналов";
			FireJournalItems = new ObservableCollection<MergeJournalItem>();
			GuardJournalItems = new ObservableCollection<MergeJournalItem>();

			ParseFromFile();
			if (fs2JournalItemsCollection != null)
			{
				Compare(fs2JournalItemsCollection);
			}
		}

		public ObservableCollection<MergeJournalItem> FireJournalItems { get; set; }
		public ObservableCollection<MergeJournalItem> GuardJournalItems { get; set; }

		void ParseFromFile()
		{
			var value = File.ReadAllText(@"C:\journal.html", Encoding.GetEncoding(1251));
			var fireIndex = value.IndexOf("Пожарные события");
			var guardIndex = value.IndexOf("Охранные события");

			if (fireIndex != -1)
			{
				var fireString = value.Substring(fireIndex, value.Length - fireIndex);
				var fireLines = fireString.Split(new string[] { "<td bgcolor=\"#75FF75\">" }, StringSplitOptions.None).ToList();
				fireLines.RemoveAt(0);

				foreach (var line in fireLines)
				{
					var mergeJournalItem = new MergeJournalItem(line);
					FireJournalItems.Add(mergeJournalItem);
				}
			}
			if (guardIndex != -1 && fireIndex != -1)
			{
				var guardString = value.Substring(guardIndex, fireIndex - guardIndex);
				var guardLines = guardString.Split(new string[] { "<td bgcolor=\"#99CCFF\">" }, StringSplitOptions.None).ToList();
				guardLines.RemoveAt(0);

				foreach (var line in guardLines)
				{
					var mergeJournalItem = new MergeJournalItem(line);
					GuardJournalItems.Add(mergeJournalItem);
				}
			}
		}

		void Compare(FS2JournalItemsCollection fs2JournalItemsCollection)
		{
			for (int i = 0; i < FireJournalItems.Count; i++)
			{
				var fs1JournalItem = FireJournalItems[i];
				var fs2JournalItem = fs2JournalItemsCollection.FireJournalItems[i];
				fs1JournalItem.Compare(fs2JournalItem);
			}

			for (int i = 0; i < GuardJournalItems.Count; i++)
			{
				var fs1JournalItem = GuardJournalItems[i];
				var fs2JournalItem = fs2JournalItemsCollection.SecurityJournalItems[i];
				fs1JournalItem.Compare(fs2JournalItem);
			}
		}
	}

	public class MergeJournalItem : BaseViewModel
	{
		public MergeJournalItem(string value)
		{
			value = value.Replace("<br>", "");
			var columns = value.Split(new string[] { "</td><td>" }, StringSplitOptions.None).ToList();

			var date = columns[1];
			var dateStrings = date.Split('.');
			var time = columns[2];
			var timeStrings = time.Split(':');
			var dateTime = new DateTime(2000 + Int32.Parse(dateStrings[2]), Int32.Parse(dateStrings[1]), Int32.Parse(dateStrings[0]), Int32.Parse(timeStrings[0]), Int32.Parse(timeStrings[1]), Int32.Parse(timeStrings[2]));

			var datalization = columns[4];
			var index = datalization.IndexOf("</td>");
			if (index != -1)
			{
				datalization = datalization.Substring(0, index);
			}
			datalization = datalization.Replace("<li>", "");
			datalization = datalization.Replace("</li>", "\n");
			if (datalization.EndsWith("\n"))
			{
				datalization = datalization.Substring(0, datalization.Length - 1);
			}

			No = columns[0].TrimStart();
			DateTime = dateTime;
			Name = columns[3];
			Detalization = datalization;
			Missmatch = "";
		}

		public void Compare(FS2JournalItem fs2JournalItem)
		{
			FS1No = fs2JournalItem.No;
			FS1DateTime = fs2JournalItem.DeviceTime;
			FS1Name = fs2JournalItem.Description;
			FS1Detalization = fs2JournalItem.Detalization;

			if (No != FS1No.ToString())
			{
				IsNoMissmatch = true;
				Missmatch += "Несовпадают номера" + "\n";
			}
			if (DateTime != FS1DateTime)
			{
				IsDateTimeMissmatch = true;
				Missmatch += "Несовпадают даты" + "\n";
			}
			if (Name != FS1Name)
			{
				IsNameMissmatch = true;
				Missmatch += "Несовпадают названия" + "\n";
			}
			if (Detalization != FS1Detalization)
			{
				IsDetalizationMissmatch = true;
				Missmatch += "Несовпадает детализация" + "\n";
			}
		}

		public string No { get; set; }
		public DateTime DateTime { get; set; }
		public string Name { get; set; }
		public string Detalization { get; set; }

		public int FS1No { get; set; }
		public DateTime FS1DateTime { get; set; }
		public string FS1Name { get; set; }
		public string FS1Detalization { get; set; }
		public string Missmatch { get; set; }
		public bool HasDifference
		{
			get { return Missmatch != ""; }
		}

		public bool IsNoMissmatch { get; set; }
		public bool IsDateTimeMissmatch { get; set; }
		public bool IsNameMissmatch { get; set; }
		public bool IsDetalizationMissmatch { get; set; }
	}
}