using System;
using System.Windows.Media;
using System.Windows.Threading;
using Common;
using Infrastructure.Common.BalloonTrayTip.ViewModels;

namespace Infrastructure.Common.BalloonTrayTip
{
	public class BalloonHelper
	{
		static BalloonToolTipViewModel balloonToolTipViewModel = new BalloonToolTipViewModel();

		public static void ShowFromAdm(string title, string text = "")
		{
			Show("[Administrator] " + title, text, Brushes.Black, Brushes.Cornsilk);
		}

		public static void ShowFromMonitor(string title, string text = "")
		{
			Show("[Monitor] " + title, text, Brushes.Black, Brushes.Cornsilk);
		}

		public static void ShowFromAgent(string title, string text = "")
		{
			Show("[Agent] " + title, text, Brushes.Black, Brushes.Cornsilk);
		}

		public static void ShowFromServer(string title, string text = "")
		{
			Show("[Server] " + title, text, Brushes.Black, Brushes.Cornsilk);
		}

		private static void Show(string title, string text = "")
		{
			Show(title, text, Brushes.Black, Brushes.Cornsilk);
		}

		public static void Show(string title, string text, Brush foregroundColor, Brush backgroundColor)
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
			{
				try
				{
					if (balloonToolTipViewModel == null)
					{
						balloonToolTipViewModel = new BalloonToolTipViewModel();
					}
					balloonToolTipViewModel.Add(title, text, foregroundColor, backgroundColor);
				}
				catch (Exception e)
				{
					Logger.Error(e, "BalloonHelper.Show");
				}
			}));
		}
	}
}