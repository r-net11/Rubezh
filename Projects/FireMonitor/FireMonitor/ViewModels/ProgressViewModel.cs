using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace FireMonitor.ViewModels
{
	public class ProgressViewModel : DialogViewModel
	{
        public bool IsShown { get; set; }

		public ProgressViewModel()
		{
            Title = "Опрос устройств";
			ProgressItems = new ObservableCollection<string>(); ;
			IsShown = false;
		}

        public void Add(string item)
        {
            if (ProgressItems.Count == 0)
                ProgressItems.Add(item);
            else
                ProgressItems.Insert(0, item);

            if (ProgressItems.Count > 100)
                ProgressItems.RemoveAt(100);

            SelectedProgressItem = ProgressItems.FirstOrDefault();
        }

		ObservableCollection<string> _progressItems;
		public ObservableCollection<string> ProgressItems
		{
			get { return _progressItems; }
			set
			{
				_progressItems = value;
				OnPropertyChanged("ProgressItems");
			}
		}

		string _selectedProgressItem;
		public string SelectedProgressItem
		{
			get { return _selectedProgressItem; }
			set
			{
				_selectedProgressItem = value;
				OnPropertyChanged("SelectedProgressItem");
			}
		}

		public override void OnClosed()
		{
			ProgressItems.Clear();
			IsShown = false;
		}
	}
}