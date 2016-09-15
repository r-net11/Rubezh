using Common;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using System;
using System.Windows.Media;
using System.Windows.Threading;
using Localization.Common.InfrastructureCommon;

namespace Infrastructure.Common.BalloonTrayTip
{
	public class BalloonHelper
	{
		private static BalloonToolTipViewModel _balloonToolTipViewModel = new BalloonToolTipViewModel();

		public static void ShowFromFiresec(string text)
		{
			Show(CommonResources.Strazh, text);
		}

		public static void ShowFromAdm(string text)
		{
			Show(CommonResources.Administrator, text);
		}

		public static void ShowFromMonitor(string text)
		{
			Show(CommonResources.FireMonitor, text);
		}

		public static void ShowFromServer(string text)
		{
			Show(CommonResources.AppServer, text);
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