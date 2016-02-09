using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhClient;
using RviClient;
using System;
using System.Net;

namespace VideoModule.ViewModels
{
	class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }
		public event EventHandler Play;
		public CameraDetailsViewModel(Camera camera = null)
		{
			Title = "Свойства камеры";
			Camera = camera ?? new Camera();
			ShowDetailsWidth = Camera.ShowDetailsWidth.ToString();
			ShowDetailsHeight = Camera.ShowDetailsHeight.ToString();
			ShowDetailsMarginLeft = Camera.ShowDetailsMarginLeft.ToString();
			ShowDetailsMarginTop = Camera.ShowDetailsMarginTop.ToString();
			_canPlay = true;
			PlayCommand = new RelayCommand(OnPlay, CanPlay);
		}
		string _showDetailsWidth;
		public string ShowDetailsWidth
		{
			get { return _showDetailsWidth; }
			set
			{
				_showDetailsWidth = value;
				OnPropertyChanged(() => ShowDetailsWidth);
			}
		}
		string _showDetailsHeight;
		public string ShowDetailsHeight
		{
			get { return _showDetailsHeight; }
			set
			{
				_showDetailsHeight = value;
				OnPropertyChanged(() => ShowDetailsHeight);
			}
		}
		string _showDetailsMarginLeft;
		public string ShowDetailsMarginLeft
		{
			get { return _showDetailsMarginLeft; }
			set
			{
				_showDetailsMarginLeft = value;
				OnPropertyChanged(() => ShowDetailsMarginLeft);
			}
		}
		string _showDetailsMarginTop;
		public string ShowDetailsMarginTop
		{
			get { return _showDetailsMarginTop; }
			set
			{
				_showDetailsMarginTop = value;
				OnPropertyChanged(() => ShowDetailsMarginTop);
			}
		}
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
		public bool PrepareToTranslation(out IPEndPoint ipEndPoint, out int vendorId)
		{
			return RviClientHelper.PrepareToTranslation(ClientManager.SystemConfiguration.RviSettings, Camera, out ipEndPoint, out vendorId);
		}
		protected override bool Save()
		{
			Camera.ShowDetailsWidth = int.Parse(ShowDetailsWidth);
			Camera.ShowDetailsHeight = int.Parse(ShowDetailsHeight);
			Camera.ShowDetailsMarginLeft = int.Parse(ShowDetailsMarginLeft);
			Camera.ShowDetailsMarginTop = int.Parse(ShowDetailsMarginTop);
			return base.Save();
		}
	}
}