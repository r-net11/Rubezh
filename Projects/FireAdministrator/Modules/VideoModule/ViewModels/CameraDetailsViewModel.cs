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
		#region <Поля>

		private bool _canPlay;

		#endregion </Поля>

		public CameraDetailsViewModel(Camera camera = null)
		{
			Title = camera == null ? "Создание нового видеоустройства" : "Свойства камеры";
			Camera = camera ?? new Camera();
			CanPlay = true;
		}

		#region <Свойства>

		public Camera Camera { get; private set; }

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

		#endregion </Свойства>
	}
}