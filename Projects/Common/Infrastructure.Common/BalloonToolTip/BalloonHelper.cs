using Common;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace Infrastructure.Common.BalloonTrayTip
{
	public class BalloonHelper
	{
		private static BalloonToolTipViewModel _balloonToolTipViewModel = new BalloonToolTipViewModel();

		public static void ShowFromFiresec(string text)
		{
			Show(Resources.Language.BalloonHelper.ShowFromFiresec, text);
		}

		public static void ShowFromAdm(string text)
		{
			Show(Resources.Language.BalloonHelper.ShowFromAdm, text);
		}

		public static void ShowFromMonitor(string text)
		{
			Show(Resources.Language.BalloonHelper.ShowFromMonitor, text);
		}

		public static void ShowFromServer(string text)
		{
			Show(Resources.Language.BalloonHelper.ShowFromServer, text);
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
					if (_balloonToolTipViewModel == null)
					{
						_balloonToolTipViewModel = new BalloonToolTipViewModel();
					}
					_balloonToolTipViewModel.Add(title, text, foregroundColor, backgroundColor);
				}
				catch (Exception e)
				{
					Logger.Error(e, "BalloonHelper.Show");
				}
			}));
		}
	}
}