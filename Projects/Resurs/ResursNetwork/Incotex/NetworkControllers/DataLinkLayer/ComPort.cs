using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;
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
        private SerialPort _SerialPort;
        /// <summary>
        /// Таймер межкадрового интервала
        /// </summary>
        private Timer _TimerInterFrameDelay;
        /// <summary>
        /// Наименование порта
        /// </summary>
        public override string PortName
        {
            get
            {
                return _SerialPort.PortName;
            }
            set
            {
                _SerialPort.PortName = value;
            }
        }
        /// <summary>
        /// Скорость передачи данных
        /// </summary>
        public int BaudRate
        {
            get { return _SerialPort.BaudRate; }
            set { _SerialPort.BaudRate = value; }
        }
        /// <summary>
        /// Кол-во бит данных во фрейме (7, 8)
        /// </summary>
        public int DataBits 
        { 
            get { return _SerialPort.DataBits; }
            set 
            {
                if ((value == 7) && (value == 8))
                {
                    _SerialPort.DataBits = value;
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
            get { return _SerialPort.StopBits; }
            set { _SerialPort.StopBits = value; }
        }
        /// <summary>
        /// Паритет фрайма
        /// </summary>
        public Parity Parity
        {
            get { return _SerialPort.Parity; }
            set { _SerialPort.Parity = value; }
        }
        /// <summary>
        /// Интервал тишины для разделения сетевых сообщений, мсек
        /// </summary>
        public Double SilentInterval
        {
            get { return _TimerInterFrameDelay.Interval; }
            set { _TimerInterFrameDelay.Interval = value; }
        }
        public override InterfaceType InterfaceType
        {
            get { return InterfaceType.ComPort; }
        }
        public override bool IsOpen
        {
            get { return _SerialPort.IsOpen; }
        } 
        #endregion

        #region Constuctors

        /// <summary>
        /// Конструктор
        /// </summary>
        public ComPort(): base()
        {
            _SerialPort = new SerialPort();
            _SerialPort.DataBits = 8;
            _SerialPort.Parity = Parity.None;
            _SerialPort.StopBits = StopBits.One;
            _SerialPort.BaudRate = 19200;
            _SerialPort.DataReceived += EventHandler_SerialPort_DataReceived;
            _SerialPort.ErrorReceived += EventHandler_SerialPort_ErrorReceived;
            _SerialPort.ReceivedBytesThreshold = 1;

            _TimerInterFrameDelay = new Timer();
            _TimerInterFrameDelay.AutoReset = false;
            _TimerInterFrameDelay.Elapsed += EventHandler_TimerInterFrameDelay_Elapsed;
            _TimerInterFrameDelay.Interval = 150;
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
            while(_SerialPort.BytesToRead != 0)
            {
                 list.Add((Byte)_SerialPort.ReadByte());
            }
            // Проверяем форат сообщения
            // Минимальная длина сообщения 1 байт: 
            //          [ADDR: 4 байта] + [CMD: 1 байт] + [DATA: 0 байт] + [CRC16: 2 байта] = 7 байт
            if (list.Count < 7)
            {
                //TODO: Ошибка. Создать служебное сообщение об ошибке
                errMessage = new ServiceErrorMessage()
                {
                    SpecificErrorCode = ErrorCode.IncorrectMessageLength,
                    Description = "Неправильная длина сообщения",
                    ExecutionTime = DateTime.Now
                };
                
                _InputBuffer.Enqueue(errMessage);
                OnMessageReceived();
            }
            // Проверяем CRC16
            var array = new Byte[list.Count - 2];
            // Получаем данные сообщения
            list.CopyTo(5, array, 0, array.Length);
            // Получаем адрес устройства
            UInt32 adr = 0;
            adr |= ((UInt32)list[3] << 24);
            adr |= ((UInt32)list[2] << 16);
            adr |= ((UInt32)list[1] << 8);
            adr |= list[0];

            message = new DataMessage(array) 
            { 
                MessageType = MessageType.IncomingMessage,
                Address = adr,
                CmdCode = list[4],
                ExecutionTime = DateTime.Now 
            };

            if ((message.CRC16.Low != list[list.Count - 2]) ||
                (message.CRC16.High != list[list.Count - 1]))
            {
                // CRC16 не совпал.
                errMessage = new ServiceErrorMessage()
                {
                    SpecificErrorCode = ErrorCode.IncorrectCRC,
                    Description = "Принято сообщение с неверной контрольной суммой",
                    ExecutionTime = DateTime.Now
                };
                _InputBuffer.Enqueue(errMessage);
                OnMessageReceived();
            }
            else
            {
                // Формируем сообщение и сохраняем его в буфер
                _InputBuffer.Enqueue(message);
                OnMessageReceived();
            }
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
            ErrorCode code;

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
            _TimerInterFrameDelay.Stop();
            _TimerInterFrameDelay.Start();
        }
        public override void Write(IMessage message)
        {
            Byte[] array = message.ToArray();
            _SerialPort.Write(array, 0, array.Length);
            // Фиксируем время отправки сообщения
            message.ExecutionTime = DateTime.Now;
        }
        /// <summary>
        /// Открывает соединение
        /// </summary>
        public override void Open()
        {
            _SerialPort.Open();
        }
        /// <summary>
        /// Закрывает соединение
        /// </summary>
        public override void Close()
        {
            _SerialPort.Close();
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
