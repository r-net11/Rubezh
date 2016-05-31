using System;
using System.Net;
using Localization.Video.ViewModels;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common;
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
			Title = CommonViewModels.CameraProperties;
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