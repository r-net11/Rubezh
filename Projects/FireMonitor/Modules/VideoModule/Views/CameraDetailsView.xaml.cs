using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Common;
using Infrastructure.Common.Windows;
using RVI.MediaSource.MediaSources;
using StrazhAPI.Models;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class CameraDetailsView
	{
	    private CancellationTokenSource _cts;

		private Camera Camera { get; set; }

		public CameraDetailsView()
		{
			InitializeComponent();

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			videoCellControl.ReconnectEvent += VideoCellControlOnReconnectEvent;
		}

		private void VideoCellControlOnReconnectEvent(object sender, EventArgs eventArgs)
		{
            videoCellControl.MediaPlayer.Close();
			StartPlaying();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			StartPlaying();
		}

		private void StartPlaying()
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;

            _cts = new CancellationTokenSource();

			Task.Factory.StartNew(() => PrepareToTranslation(viewModel, _cts.Token), _cts.Token);
		}

	    private void PrepareToTranslation(CameraDetailsViewModel viewModel, CancellationToken cancellationToken)
	    {
	        try
	        {
	            IPEndPoint ipEndPoint;
	            int vendorId;
	            Logger.Info(string.Format("Камера '{0}'. Попытка начать трансляцию.", viewModel.Camera.Name));
	            while (!viewModel.PrepareToTranslation(out ipEndPoint, out vendorId))
	            {
	                cancellationToken.ThrowIfCancellationRequested();
	                Thread.Sleep(10000);
	                Logger.Info(string.Format("Камера '{0}'. Очередная попытка начать трансляцию.", viewModel.Camera.Name));
	            }
	            ApplicationService.Invoke(() =>
	            {
                    Logger.Info(
	                    string.Format(
	                        "Камера '{0}'. Реквизиты для начала трансляции получены. Адрес='{1}', Издатель='{2}'",
	                        viewModel.Camera.Name, ipEndPoint, vendorId));

	                videoCellControl.MediaPlayer.Open(MediaSourceFactory.CreateFromServerOnlineStream(ipEndPoint, vendorId));
	                Logger.Info(string.Format("Камера '{0}'. Старт трансляции.", viewModel.Camera.Name));
	                videoCellControl.MediaPlayer.Play();
	            });
	        }
	        catch (OperationCanceledException)
	        {
                Logger.Info(string.Format("Камера '{0}'. Прервана попытка начать трансляцию.", viewModel.Camera.Name));
            }
	        catch (Exception e)
	        {
	            Logger.Error(e,
	                string.Format("Камера '{0}'. Исключительная ситуация при попытке трансляции.", viewModel.Camera.Name));
	            ApplicationService.Invoke(() =>
	            {
	                videoCellControl.ShowReconnectButton = true;
	            });
	        }
	    }

	    private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
	    {
	        CancelPrepareToTranslation();
			StopPlaying();

			videoCellControl.ReconnectEvent -= VideoCellControlOnReconnectEvent;
			Loaded -= OnLoaded;
			Unloaded -= OnUnloaded;
			Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
		}

		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
		    CancelPrepareToTranslation();
			StopPlaying();
		}

	    private void CancelPrepareToTranslation()
	    {
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }

	    private void StopPlaying()
	    {
            videoCellControl.MediaPlayer.Stop();
            videoCellControl.MediaPlayer.Close();
        }
	}
}