using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhClient;
using RviClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace VideoModule.ViewModels
{
	class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }
		public event EventHandler Play;
		public event EventHandler Stop;
		public CameraDetailsViewModel(Camera camera)
		{
			Title = "Свойства камеры";
			Camera = camera;
			RviStreams = new List<RviStreamViewModel>();
			camera.RviStreams.ForEach(rviStream => RviStreams.Add(new RviStreamViewModel(rviStream)));
			SelectedRviStream = RviStreams.FirstOrDefault(x => x.StreamNumber == camera.SelectedRviStreamNumber);
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
		public List<RviStreamViewModel> RviStreams { get; private set; }
		private RviStreamViewModel _selectedRviStream;
		public RviStreamViewModel SelectedRviStream
		{
			get { return _selectedRviStream; }
			set
			{
				_selectedRviStream = value;
				OnStop();
				OnPlay();
				OnPropertyChanged(() => SelectedRviStream);
			}
		}
		public RelayCommand PlayCommand { get; private set; }
		void OnPlay()
		{
			if (Play != null)
				Play(this, EventArgs.Empty);
			_canPlay = false;
		}
		bool _canPlay;
		bool CanPlay()
		{
			return _canPlay;
		}
		void OnStop()
		{
			if (Stop != null)
				Stop(this, EventArgs.Empty);
		}
		public bool PrepareToTranslation(out IPEndPoint ipEndPoint, out int vendorId)
		{
			var selectedRviStream = Camera.RviStreams.FirstOrDefault(x => x.Number == SelectedRviStream.StreamNumber);
			return RviClientHelper.PrepareToTranslation(ClientManager.SystemConfiguration.RviSettings, selectedRviStream, out ipEndPoint, out vendorId);
		}
		protected override bool Save()
		{
			Camera.ShowDetailsWidth = int.Parse(ShowDetailsWidth);
			Camera.ShowDetailsHeight = int.Parse(ShowDetailsHeight);
			Camera.ShowDetailsMarginLeft = int.Parse(ShowDetailsMarginLeft);
			Camera.ShowDetailsMarginTop = int.Parse(ShowDetailsMarginTop);
			Camera.SelectedRviStreamNumber = SelectedRviStream.StreamNumber;
			return base.Save();
		}
	}
}