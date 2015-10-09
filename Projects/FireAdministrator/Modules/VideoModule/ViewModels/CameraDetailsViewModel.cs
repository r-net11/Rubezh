using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }

		private bool _canPlay;
		public bool CanPlay
		{
			get { return _canPlay; }
			set
			{
				if (_canPlay == value)
					return;
				_canPlay = value;
				OnPropertyChanged(() => CanPlay);
			}
		}

		public CameraDetailsViewModel(Camera camera = null)
		{
			Title = "Свойства камеры";
			Camera = camera ?? new Camera();
			CanPlay = true;
		}
	}
}