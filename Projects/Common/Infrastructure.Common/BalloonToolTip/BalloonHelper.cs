using Common;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace Infrastructure.Common.BalloonTrayTip
{
	public class BalloonHelper
	{
		private static BalloonToolTipViewModel balloonToolTipViewModel = new BalloonToolTipViewModel();
		private static Brush ForegroundColor = Brushes.Black;

		public static void ShowFromFiresec(string text)
		{
			Show("Глобал", text);
		}

		public static void ShowFromAdm(string text)
		{
			Show("Администратор", text);
		}

		public static void ShowFromMonitor(string text)
		{
			Show("Оперативная задача", text);
		}

		public static void ShowFromServer(string text)
		{
			Show("Сервер приложений", text);
		}

		private static void Show(string title, string text)
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