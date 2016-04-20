using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Common;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.BalloonTrayTip.ViewModels
{
	public class BalloonToolTipViewModel : WindowBaseViewModel
	{
		public static bool IsEmpty
		{
			get { return Items.Count == 0; }
		}

		static Views.BalloonToolTipView customBalloonView;

		static List<Item> Items = new List<Item>();

		public Item LastItem
		{
			get { return Items.LastOrDefault(); }
		}

		public BalloonToolTipViewModel(string title, string text, Brush foregroundColor, Brush backgroundColor)
		{
			Items.Clear();
			Title = "";
			Add(title, text, foregroundColor, backgroundColor);
			RemoveItemCommand = new RelayCommand(OnRemoveItem);
			ClearCommand = new RelayCommand(OnClear);
			customBalloonView = new Views.BalloonToolTipView();
			customBalloonView.DataContext = this;
		}

		public BalloonToolTipViewModel()
		{
			Title = "";
			RemoveItemCommand = new RelayCommand(OnRemoveItem);
			ClearCommand = new RelayCommand(OnClear);
			customBalloonView = new Views.BalloonToolTipView();
			customBalloonView.DataContext = this;
		}

		public void Add(string title, string text, Brush foregroundColor, Brush backgroundColor)
		{
			Items.Add(new Item
			{
				Title = title,
				Text = text,
				ForegroundColor = foregroundColor,
				BackgroundColor = backgroundColor,
			});
			OnPropertyChanged(() => LastItem);
		}

		public RelayCommand RemoveItemCommand { get; private set; }
		private void OnRemoveItem()
		{
			try
			{
				Items.Remove(Items.LastOrDefault());
				if (Items.Count != 0)
				{
					OnPropertyChanged(() => LastItem);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Balloon.RemoveItem");
			}
		}

		public RelayCommand ClearCommand { get; private set; }
		private void OnClear()
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