using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;
using ResursNetwork.Modbus;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.Messages;
using ResursNetwork.Incotex.NetworkControllers;
using ResursNetwork.Incotex.NetworkControllers.Messages;

namespace ResursNetwork.Incotex.NetworkControllers.DataLinkLayer
{
    /// <summary>
    /// Объет для передачи и приёма сообщений по физическому
    /// интерфейсу RS232 и RS485
    /// </summary>
    public class ComPort : DataLinkPortBase
    {
        #region Fields And Properties
        /// <summary>
        /// COM-порт
        /// </summary>
        private SerialPort _serialPort;
        /// <summary>
        /// Таймер межкадрового интервала
        /// </summary>
        private Timer _timerInterFrameDelay;

        /// <summary>
        /// Наименование порта
        /// </summary>
        public override string PortName
        {
            get
            {
                return _serialPort.PortName;
            }
            set
            {
                _serialPort.PortName = value;
            }
        }

        /// <summary>
        /// Скорость передачи данных
        /// </summary>
        public int BaudRate
        {
            get { return _serialPort.BaudRate; }
            set { _serialPort.BaudRate = value; }
        }

        /// <summary>
        /// Кол-во бит данных во фрейме (7, 8)
        /// </summary>
        public int DataBits 
        { 
            get { return _serialPort.DataBits; }
            set 
            {
                if ((value == 7) && (value == 8))
                {
                    _serialPort.DataBits = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Кол-во стоп битов во фрайме
        /// </summary>
        public StopBits StopBits
        {
            get { return _serialPort.StopBits; }
            set { _serialPort.StopBits = value; }
        }

        /// <summary>
        /// Паритет фрайма
        /// </summary>
        public Parity Parity
        {
            get { return _serialPort.Parity; }
            set { _serialPort.Parity = value; }
        }

        /// <summary>
        /// Интервал тишины для разделения сетевых сообщений, мсек
        /// </summary>
        public Double SilentInterval
        {
            get { return _timerInterFrameDelay.Interval; }
            set { _timerInterFrameDelay.Interval = value; }
        }

        public override InterfaceType InterfaceType
        {
            get { return InterfaceType.ComPort; }
        }

        public override bool IsOpen
        {
            get { return _serialPort.IsOpen; }
        } 

        #endregion

        #region Constuctors

        /// <summary>
        /// Конструктор
        /// </summary>
        public ComPort(): base()
        {
            _serialPort = new SerialPort();
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.BaudRate = 19200;
            _serialPort.DataReceived += EventHandler_SerialPort_DataReceived;
            _serialPort.ErrorReceived += EventHandler_SerialPort_ErrorReceived;
            _serialPort.ReceivedBytesThreshold = 1;

            _timerInterFrameDelay = new Timer();
            _timerInterFrameDelay.AutoReset = false;
            _timerInterFrameDelay.Elapsed += EventHandler_TimerInterFrameDelay_Elapsed;
            _timerInterFrameDelay.Interval = 150;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Обработчик срабатываения таймера межкадрового интервала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TimerInterFrameDelay_Elapsed(
            object sender, ElapsedEventArgs e)
        {
            DataMessage message;
            ServiceErrorMessage errMessage;
            // Если сработал межкадровый таймер, значит сообщение полностью
            // принято.
            List<Byte> list = new List<byte>();
            // Получаем массив байт принятого сообщения
            while(_serialPort.BytesToRead != 0)
            {
                 list.Add((Byte)_serialPort.ReadByte());
            }
            
			// Проверяем форат сообщения
            // Минимальная длина сообщения 1 байт: 
            //          [ADDR: 4 байта] + [CMD: 1 байт] + [DATA: 0 байт] + [CRC16: 2 байта] = 7 байт
            if (list.Count < 7)
            {
                //TODO: Ошибка. Создать служебное сообщение об ошибке
                errMessage = new ServiceErrorMessage
                {
                    SpecificErrorCode = ErrorCode.IncorrectMessageLength,
                    Description = "Неправильная длина сообщения",
                    ExecutionTime = DateTime.Now
                };
                
                _InputBuffer.Enqueue(errMessage);
                OnMessageReceived();
				return;
            }

            // Проверяем CRC16
            var array = new Byte[list.Count - 2];
			list.CopyTo(0, array, 0, array.Length);
			if (!CRC16.CheckCRC16(list.ToArray()))
			{
				errMessage = new ServiceErrorMessage
				{
					SpecificErrorCode = ErrorCode.IncorrectCRC,
					Description = "Неправильная контрольная сумма",
					ExecutionTime = DateTime.Now
				};

				_InputBuffer.Enqueue(errMessage);
				OnMessageReceived();
				return;
			}

            // Получаем данные сообщения
			array = new Byte[list.Count - 7]; // 7 = 5 [adr:4 cmd: 1] + 2 [crc16: 2]
            list.CopyTo(5, array, 0, array.Length);

			// Получаем адрес устройства
            UInt32 adr = 0;
			//adr |= ((UInt32)list[3] << 24);
			//adr |= ((UInt32)list[2] << 16);
			//adr |= ((UInt32)list[1] << 8);
			//adr |= list[0];

			var arrayAdr = new Byte[4];
			list.CopyTo(0, arrayAdr, 0, 4);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(arrayAdr);
			adr = BitConverter.ToUInt32(arrayAdr, 0); 

            message = new DataMessage(array) 
            { 
                MessageType = MessageType.IncomingMessage,
                Address = adr,
                CmdCode = list[4],
                ExecutionTime = DateTime.Now 
            };
			
			// Формируем сообщение и сохраняем его в буфер
			_InputBuffer.Enqueue(message);
			OnMessageReceived();
        }

        /// <summary>
        /// Обработчик события возникновения ошибки при работе порта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_SerialPort_ErrorReceived(
            object sender, SerialErrorReceivedEventArgs e)
        {
            //Ошибка. Создать служебное сообщение об ошибке
            ErrorCode code = ErrorCode.UnknownError;

            switch (e.EventType)
            {
                case SerialError.Frame:
                    { code = ErrorCode.ErrorSerialPortFrame; break; }
                case SerialError.Overrun:
                    { code = ErrorCode.ErrorSerialPortOverrun; break; }
                case SerialError.RXOver:
                    { code = ErrorCode.ErrorSerialPortRXOver; break; }
                case SerialError.RXParity:
                    { code = ErrorCode.ErrorSerialPortRXParity; break; }
                case SerialError.TXFull:
                    { code = ErrorCode.ErrorSerialPortTXFull; break; }
                default:
                    { code = ErrorCode.UnknownError; break; }
            }

            var errMessage = new ServiceErrorMessage()
            {
                SpecificErrorCode = ErrorCode.ErrorSerialPortFrame,
                Description = "Ошибка COM-порта",
                ExecutionTime = DateTime.Now
            };

            _InputBuffer.Enqueue(errMessage);
            OnMessageReceived();
        }

        /// <summary>
        /// Обработчик события получения данных из сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_SerialPort_DataReceived(
            object sender, SerialDataReceivedEventArgs e)
        {
            // Сбрасываем межкадровый таймер при приёме очередных данных
            // и запускаем его снова
            _timerInterFrameDelay.Stop();
            _timerInterFrameDelay.Start();
        }

        public override void Write(IMessage message)
        {
            Byte[] array = message.ToArray();
            _serialPort.Write(array, 0, array.Length);
            // Фиксируем время отправки сообщения
            message.ExecutionTime = DateTime.Now;
        }

        /// <summary>
        /// Открывает соединение
        /// </summary>
        public override void Open()
        {
            _serialPort.Open();
        }

        /// <summary>
        /// Закрывает соединение
        /// </summary>
        public override void Close()
        {
            _serialPort.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(
                "Type={0}; PortName={1}; ControllerId={2}; BaudRate={3}; " +
                "DataBits={4}; StopBits={5}; Parity={6}; SilentInterval={7}",
                this.GetType().ToString(), PortName,
                _NetworkController == null ? String.Empty : _NetworkController.Id.ToString(),
                BaudRate, DataBits, StopBits, Parity, SilentInterval);
            //return base.ToString();
        }

        #endregion
    }
}
