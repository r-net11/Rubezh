using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RviClient;

namespace VideoModule.ViewModels
{
	class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }

		public event EventHandler Play;

		protected virtual void RaisePlay()
		{
			var temp = Play;
			if (temp != null)
				temp(this, EventArgs.Empty);
		}

		private bool _canPlay;

		public RelayCommand PlayCommand { get; private set; }
		private void OnPlay()
		{
			RaisePlay();
			_canPlay = false;
		}
		public bool CanPlay()
		{
			return _canPlay;
		}

		public CameraDetailsViewModel(Camera camera = null)
		{
			Title = "Свойства камеры";
			Camera = camera ?? new Camera();
			_canPlay = true;
			PlayCommand = new RelayCommand(OnPlay, CanPlay);
		}

		public bool PrepareToTranslation(out IPEndPoint ipEndPoint, out int vendorId)
		{
			return RviClientHelper.PrepareToTranslation(FiresecManager.SystemConfiguration, Camera, out ipEndPoint, out vendorId);
		}
	}
}