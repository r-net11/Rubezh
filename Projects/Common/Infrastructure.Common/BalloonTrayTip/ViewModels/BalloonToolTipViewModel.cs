using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Common;
using System.Windows.Media;


namespace Infrastructure.Common.BalloonTrayTip.ViewModels
{
	public class BalloonToolTipViewModel : WindowBaseViewModel
	{
		public static bool IsEmpty = false;

		List<Item> Items = new List<Item>();
		public Item LastItem
		{
			get { return Items.LastOrDefault(); }
		}

		public void AddNote(string title, string text, Brush foregroundColor, Brush backgroundColor)
		{
			Items.Add(new Item { Title = title, Text = text, ForegroundColor = foregroundColor, BackgroundColor = backgroundColor });
			OnPropertyChanged("LastItem");
		}

		public BalloonToolTipViewModel(string title, string text, Brush foregroundColor, Brush backgroundColor)
		{
			Items.Clear();
			Title = "";
			AddNote(title, text, foregroundColor, backgroundColor);
			OnPropertyChanged("LastItem");
			RemoveItemCommand = new RelayCommand(OnRemoveItem);
			ClearCommand = new RelayCommand(OnClear);
		}
		public BalloonToolTipViewModel()
		{
			Title = "";
			RemoveItemCommand = new RelayCommand(OnRemoveItem);
			ClearCommand = new RelayCommand(OnClear);
		}

		public RelayCommand RemoveItemCommand { get; private set; }
		void OnRemoveItem()
		{
			try
			{
				Items.Remove(Items.LastOrDefault());
				if (Items.Count == 0)
				{
                    IsEmpty = true;
				}
				else
				{
					OnPropertyChanged("LastItem");
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Balloon.RemoveItem");
			}

		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			try
			{
				Items.Clear();
				this.Close();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Balloon.ClearItem");
			}
		}

		public class Item
		{
			public string Title { get; set; }
			public string Text { get; set; }
			public System.Windows.Media.Brush ForegroundColor { get; set; }
			public System.Windows.Media.Brush BackgroundColor { get; set; }
		}
	}
}