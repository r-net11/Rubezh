using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Common;
using Infrastructure.Common.Windows;
using RVI.Client.Retranslator;
using RVI.Client.Retranslator.Interfaces;
using RVI.MediaSource.MediaSources;
using RVI.MediaSource.MediaSources.RetranslatorStreams;
using RVI.MediaSource.MediaSources.StreamSource;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class CameraDetailsView : UserControl
	{
		public CameraDetailsView()
		{
			InitializeComponent();

			DataContextChanged += OnDataContextChanged;
			Unloaded += OnUnloaded;
			videoCellControl.ReconnectEvent += VideoCellControlOnReconnectEvent;
		}

		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;

			viewModel.Play -= ViewModelOnPlay;
			viewModel.Play += ViewModelOnPlay;
		}

		private void ViewModelOnPlay(object sender, EventArgs eventArgs)
		{
			StartPlaying();
		}

		private void StartPlaying()
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;


			Task.Factory.StartNew(() =>
			{
				try
				{
					IPEndPoint ipEndPoint;
					int vendorId;
					Logger.Info(string.Format("Камера '{0}'. Попытка начать трансляцию.", viewModel.Camera.Name));
					while (!viewModel.PrepareToTranslation(out ipEndPoint, out vendorId))
					{
						Thread.Sleep(30000);
						Logger.Info(string.Format("Камера '{0}'. Очередная попытка начать трансляцию.", viewModel.Camera.Name));
					}
					ApplicationService.Invoke(() =>
					{
						Logger.Info(string.Format("Камера '{0}'. Реквизиты для начала трансляции получены. Адрес='{1}', Издатель='{2}'", viewModel.Camera.Name, ipEndPoint, vendorId));
						videoCellControl.MediaPlayer.Open(new StreamMediaSource(new RetranslatorStream(vendorId, new InternalReader(ipEndPoint)), 0));
						Logger.Info(string.Format("Камера '{0}'. Старт трансляции.", viewModel.Camera.Name));
						videoCellControl.MediaPlayer.Play();
                    });
				}
				catch (Exception e)
				{
					Logger.Error(e, string.Format("Камера '{0}'. Исключительная ситуация при попытке трансляции.", viewModel.Camera.Name));
					ApplicationService.Invoke(() =>
					{
						videoCellControl.ShowReconnectButton = true;
					});
				}
			});
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			videoCellControl.ReconnectEvent -= VideoCellControlOnReconnectEvent;

			videoCellControl.MediaPlayer.Stop();
			videoCellControl.MediaPlayer.Close();

			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;

			viewModel.Play -= ViewModelOnPlay;
		}

		private void VideoCellControlOnReconnectEvent(object sender, EventArgs eventArgs)
		{
			StartPlaying();
		}
	}


    internal class InternalReader : IRetranslatorStreamReader
    {
        #region события

        /// <summary>
        /// Чтение потока из сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void clientStreamer_OnReadBuffer(object sender, StreamerBufferEventArgs args)
        {
            RaiseOnReadBuffer(args.Buffer);
        }

        /// <summary>
        /// Событие чтения потока из сети
        /// </summary>
        /// <param name="buffer"></param>
        protected virtual void RaiseOnReadBuffer(byte[] buffer)
        {
            var handler = OnReadBuffer;
            if (handler != null)
            {
                handler(this, new StreamerBufferEventArgs(buffer));
            }
        }

        #endregion

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            _clientStreamer.OnReadBuffer += clientStreamer_OnReadBuffer;
            _clientStreamer.StartWait();
        }

        public void Stop()
        {
            _clientStreamer.Stop();
        }

        public void IncreaseSpeed()
        {
            throw new NotImplementedException();
        }

        public void DecreaseSpeed()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<StreamerBufferEventArgs> OnReadBuffer;

        private IPEndPoint ipEndPoint;
        private TcpStreamClient _clientStreamer;

        public InternalReader(IPEndPoint ipEndPoint)
        {
            this.ipEndPoint = ipEndPoint;
            _clientStreamer = new TcpStreamClient(ipEndPoint, CancellationToken.None);
        }
    }

    /// <summary>
    /// Получение потока с сервера по TCP
    /// </summary>
    internal class TcpStreamClient : BaseClientStreamer
    {
        #region поля и свойства

        /// <summary>
        /// Клиент TCP
        /// </summary>
        private readonly TcpClient _tcpClient;

        /// <summary>
        /// Поток данных из сети
        /// </summary>
        private NetworkStream _networkStream;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ipEndPoint">IP + порт</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public TcpStreamClient(IPEndPoint ipEndPoint, CancellationToken cancellationToken)
            : base(ipEndPoint, cancellationToken)
        {
            _tcpClient = new TcpClient();
        }

        #endregion

        #region методы

        /// <summary>
        /// Начать трансляцию
        /// </summary>
        protected override void StartInternal()
        {
            _tcpClient.Connect(_ipEndPoint);
            _networkStream = _tcpClient.GetStream();
        }

        /// <summary>
        /// Остановить трансляцию
        /// </summary>
        protected override void StopInternal()
        {
            if (_networkStream != null)
            {
                _networkStream.Close();
            }

            _tcpClient.Close();
        }

        /// <summary>
        /// Получение потока из сети
        /// </summary>
        protected override void ReceiveDataInternal()
        {
            if (_networkStream == null || !_networkStream.CanRead)
            {
                return;
            }

            //прочитанный буфер может быть меньше выделененого, поэтому создаем локальный
            int readCount = _networkStream.Read(_buffer, 0, _buffer.Length);
            if (readCount > 0)
            {
                var localBuffer = new byte[readCount];
                Array.Copy(_buffer, localBuffer, readCount);
                RaiseOnReadBuffer(localBuffer);
                localBuffer = null;
                Logger.Info(".");
            }
        }

        #endregion
    }

    /// <summary>
    /// Абстрактные класс работы с потоком ретранслятора
    /// </summary>
    internal abstract class BaseClientStreamer
    {
        #region поля и свойства

        /// <summary>
        /// true - если запущено
        /// </summary>
        private volatile bool _isStarted;

        /// <summary>
        /// true - если в процессе остановки
        /// </summary>
        protected volatile bool _isStopping;

        /// <summary>
        /// IP + порт
        /// </summary>
        protected readonly IPEndPoint _ipEndPoint;

        /// <summary>
        /// Токен отмены
        /// </summary>
        protected readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Буфер чтения с сервера. Значение выставлено как на сервере для udp.
        /// </summary>
        protected byte[] _buffer = new byte[65000];

        /// <summary>
        /// Событие чтения потока из сети
        /// </summary>
        public EventHandler<StreamerBufferEventArgs> OnReadBuffer;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ipEndPoint">IP + порт</param>
        /// <param name="cancellationToken">Токен отмены</param>
        protected BaseClientStreamer(IPEndPoint ipEndPoint, CancellationToken cancellationToken)
        {
            _ipEndPoint = ipEndPoint;
            _cancellationToken = cancellationToken;
        }

        #endregion

        #region методы

        /// <summary>
        /// Получение потока с сервера
        /// </summary>
        public void StartWait()
        {
            try
            {
                if (_isStarted)
                {
                    return;
                }
                _isStarted = true;
                _isStopping = false;

                StartInternal();

                while (_isStarted)
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    ReceiveDataInternal();
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SocketException)
            {

            }
            catch (ObjectDisposedException)
            {

            }
            catch (IOException)
            {

            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Logger.Error(e.StackTrace);
                throw;
            }
            finally
            {
                Stop();
            }
        }

        /// <summary>
        /// Остановить трансляцию
        /// </summary>
        public void Stop()
        {
            try
            {
                if (!_isStarted || _isStopping)
                {
                    return;
                }

                _isStopping = true;


                StopInternal();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Logger.Error(e.StackTrace);
            }
            finally
            {
                _isStarted = false;
                _isStopping = false;
            }
        }

        #endregion

        #region abstract

        /// <summary>
        /// Начать трансляцию
        /// </summary>
        protected abstract void StartInternal();

        /// <summary>
        /// Остановить трансляцию
        /// </summary>
        protected abstract void StopInternal();

        /// <summary>
        /// Получение потока из сети
        /// </summary>
        protected abstract void ReceiveDataInternal();

        #endregion

        #region protected

        /// <summary>
        /// Событие чтения потока из сети
        /// </summary>
        /// <param name="buffer"></param>
        protected void RaiseOnReadBuffer(byte[] buffer)
        {
            if (_isStopping)
            {
                return;
            }

            var handler = OnReadBuffer;
            if (handler != null)
            {
                handler(this, new StreamerBufferEventArgs(buffer));
            }
        }

        #endregion
    }

    internal class RetranslatorStream : IRetranslatorStream
    {
        #region поля и свойства

        /// <summary>
        /// Чтение потока из сети по udp или tcp
        /// </summary>
        private readonly IRetranslatorStreamReader _streamer;

        /// <summary>
        /// Примитив блокировки
        /// </summary>
        private readonly object _eventHandlerLockObject = new object();

        /// <summary>
        /// Начальные байты
        /// </summary>
        private byte[] _initBytes;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="vendorId">Id вендора</param>
        /// <param name="streamer">Чтение потока из сети</param>
        public RetranslatorStream(int vendorId, IRetranslatorStreamReader streamer)
        {
           // streamer.ThrowIfArgumentIsNull("streamer");

            this.DeviceVendorType = vendorId;
            this._streamer = streamer;
        }

        #endregion

        #region методы

        /// <summary>
        /// Запусть трансляцию потока
        /// </summary>
        private void StartReceive()
        {
            this._streamer.OnReadBuffer += streamer_OnReadBuffer;
            _streamer.Start();
        }

        /// <summary>
        /// Закрыть получение потока
        /// </summary>
        private void Close()
        {
            this._streamer.OnReadBuffer -= streamer_OnReadBuffer;
            _streamer.Stop();
        }

        #endregion

        #region события

        /// <summary>
        /// Чтение потока из сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void streamer_OnReadBuffer(object sender, StreamerBufferEventArgs args)
        {
            var buffer = args.Buffer;
            var dataPtr = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(args.Buffer, 0, dataPtr, buffer.Length);
            RaiseEventHandler(this, new FrameDataEventArgs(dataPtr, (uint)buffer.Length));
            Marshal.FreeHGlobal(dataPtr);
        }

        /// <summary>
        /// Делагаты для обработки события поступления сырых данных от устройства
        /// </summary>
        private EventHandler<FrameDataEventArgs> _eventHandler;

        /// <summary>
        /// Подписка на событие
        /// </summary>
        public event EventHandler<FrameDataEventArgs> NewFrameData
        {
            add
            {
                lock (_eventHandlerLockObject)
                {
                    var d = _eventHandler;
                    if (_eventHandler == null)
                    {
                        _eventHandler = (EventHandler<FrameDataEventArgs>)Delegate.Combine(_eventHandler, value);

                    }
                    else
                    {
                        if (!_eventHandler.GetInvocationList().Contains(value))
                        {
                            _eventHandler = (EventHandler<FrameDataEventArgs>)Delegate.Combine(_eventHandler, value);
                        }
                    }
                    if (_eventHandler.GetInvocationList().Count() == 1)
                    {
                        StartReceive();
                    }
                }
            }
            remove
            {
                lock (_eventHandlerLockObject)
                {
                    var d = _eventHandler;

                    if ((d == null) || (!d.GetInvocationList().Contains(value)))
                    {
                        return;
                    }
                    _eventHandler = (EventHandler<FrameDataEventArgs>)Delegate.Remove(d, value);

                    if (_eventHandler == null)
                    {
                        Close();
                        return;
                    }

                    if (!_eventHandler.GetInvocationList().Any())
                    {
                        Close();
                    }
                }
            }
        }

        /// <summary>
        /// Вброс события
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RaiseEventHandler(Object sender, FrameDataEventArgs e)
        {
            EventHandler<FrameDataEventArgs> d;
            lock (_eventHandlerLockObject)
            {
                d = _eventHandler;
            }

            if (d != null)
            {
                // Because the dictionary can contain several different delegate types,
                // it is impossible to construct a type­safe call to the delegate at 
                // compile time. So, I call the System.Delegate type's DynamicInvoke 
                // method, passing it the callback method's parameters as an array of 
                // objects. Internally, DynamicInvoke will check the type safety of the 
                // parameters with the callback method being called and call the method.
                // If there is a type mismatch, then DynamicInvoke will throw an exception.
                d(sender, e);
            }
        }

        #endregion

        #region IRetranslatorStream

        /// <summary>
        /// Тип вендора
        /// </summary>
        public int DeviceVendorType
        {
            get;
            set;
        }

        /// <summary>
        /// Ускорить воспроизведение трансляции потока для клиента
        /// </summary>
        public void IncreaseSpeed()
        {
            _streamer.IncreaseSpeed();
        }

        /// <summary>
        /// Замедлить воспроизведение трансляции потока для клиента
        /// </summary>
        public void DecreaseSpeed()
        {
            _streamer.DecreaseSpeed();
        }

        /// <summary>
        /// Остановка трасляции
        /// </summary>
        public event EventHandler OnStopTranslator;

        #endregion <IStream>
    }


}