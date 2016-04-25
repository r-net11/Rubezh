using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService
{
	static class PageController
	{
		static object locker = new object();
		static Page _currentPage;
		static public Page CurrentPage
		{
			get { return _currentPage; }
			set
			{
				if (_currentPage != value)
				{
					_currentPage = value;
					Redraw();
				}

			}
		}

		static Dictionary<Page, ConsolePageBase> Pages { get; set; }

		static ConnectionsPage ConnectionsPage { get; set; }
		static LogPage LogPage { get; set; }
		static StatusPage StatusPage { get; set; }
		static GKPage GKPage { get; set; }
		static PollingPage PollingPage { get; set; }
		static OperationsPage OperationsPage { get; set; }
		static LicensePage LicensePage { get; set; }

		const int HeaderHeight = 3;
		const int FooterHeight = 0;


		static PageController()
		{
			ConnectionsPage = new ConnectionsPage();
			LogPage = new LogPage();
			StatusPage = new StatusPage();
			GKPage = new GKPage();
			PollingPage = new PollingPage();
			OperationsPage = new OperationsPage();
			LicensePage = new LicensePage();
			CurrentPage = Page.Connections;
			Pages = new Dictionary<Page, ConsolePageBase>
			{
				{Page.Connections, ConnectionsPage},
				{Page.Log, LogPage},
				{Page.Status, StatusPage},
				{Page.GK, GKPage},
				{Page.Polling, PollingPage},
				{Page.Operations, OperationsPage},
				{Page.License, LicensePage}
			};
		}

		public static void OnKeyPressed(ConsoleKey key)
		{
			switch (key)
			{
				case ConsoleKey.F1: CurrentPage = Page.Connections; return;
				case ConsoleKey.F2: CurrentPage = Page.Log; return;
				case ConsoleKey.F3: CurrentPage = Page.Status; return;
				case ConsoleKey.F4: CurrentPage = Page.GK; return;
				case ConsoleKey.F5: CurrentPage = Page.Polling; return;
				case ConsoleKey.F6: CurrentPage = Page.Operations; return;
				case ConsoleKey.F7: CurrentPage = Page.License; return;
				case ConsoleKey.LeftArrow:
					{
						var index = Pages.Keys.ToList().IndexOf(CurrentPage);
						if (index > 0)
							CurrentPage = Pages.Keys.ElementAt(index - 1);
						return;
					}
				case ConsoleKey.RightArrow:
					{
						var index = Pages.Keys.ToList().IndexOf(CurrentPage);
						if (index < Pages.Count - 1)
							CurrentPage = Pages.Keys.ElementAt(index + 1);
						return;
					}
				default:
					Pages[CurrentPage].OnKeyPressed(key);
					return;
			}
		}

		public static void OnPageChanged(Page page)
		{
			if (page == CurrentPage)
				lock (locker)
					Pages[page].Draw(0, HeaderHeight, Console.WindowWidth, Console.WindowHeight - HeaderHeight - FooterHeight);
		}

		public static void Redraw()
		{
			Console.ResetColor();
			Console.Clear();

			DrawHeader();
			Pages[CurrentPage].Draw(0, HeaderHeight, Console.WindowWidth, Console.WindowHeight - HeaderHeight - FooterHeight);
		}


		static void SetColors(Page page)
		{
			if (CurrentPage == page)
			{
				Console.BackgroundColor = ColorTheme.BackgroundColor;
				Console.ForegroundColor = ColorTheme.ForegroundColor;
			}
			else
			{
				Console.BackgroundColor = ColorTheme.InactiveBackgroundColor;
				Console.ForegroundColor = ColorTheme.InactiveForegroundColor;
			}
		}

		static void DrawHeader()
		{
			var width = Console.WindowWidth;
			foreach (var page in Pages)
			{
				SetColors(page.Key);
				ConsoleHelper.Write(page.Value.Name.Length + 2);
			}
			Console.BackgroundColor = ColorTheme.InactiveBackgroundColor;
			ConsoleHelper.WriteToEnd(width);
			foreach (var page in Pages)
			{
				SetColors(page.Key);
				Console.Write(" {0} ", page.Value.Name);
			}
			Console.BackgroundColor = ColorTheme.InactiveBackgroundColor;
			ConsoleHelper.WriteToEnd(width);
			foreach (var page in Pages)
			{
				SetColors(page.Key);
				ConsoleHelper.Write(page.Value.Name.Length + 2);
			}
			Console.BackgroundColor = ColorTheme.InactiveBackgroundColor;
			ConsoleHelper.WriteToEnd(width);
		}
	}

	public enum Page
	{
		Connections,
		Log,
		Status,
		GK,
		Polling,
		Operations,
		License
	}
}
