using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transactions;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.NetworkControllers.Messages;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.Management;
using ResursAPI.Models;
using ResursAPI.ParameterNames;
using ResursAPI.CommandNames;
using Common;

namespace ResursNetwork.Incotex.Models
{
    /// <summary>
    /// Модель данных счётчика Меркурий M203
    /// </summary>
    public class Mercury203: DeviceBase
    {
        #region Fields And Properties
        
		/// <summary>
        /// Хранит все активные операции по данному устройтву
        /// </summary>
        List<NetworkRequest> _activeRequests = new List<NetworkRequest>();

        public override DeviceModel DeviceModel
        {
            get { return DeviceModel.Mercury203; }
        }

		public override uint Address
		{
			get { return base.Address; }
			set
			{
				if (value != 0)
				{
					base.Address = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException(
						"Попытка установить недопустимый адрес равеный 0");
				}
			}
		}

		public uint GroupAddress
		{
			get 
			{ 
				return Convert.ToUInt32(_parameters[ParameterNamesMercury203.GADDR].Value); 
			}
			set
			{
				if (value == 0)
				{
					throw new ArgumentOutOfRangeException(
						"Групповой адрес должен быть больше 0");
				}
				_parameters[ParameterNamesMercury203.GADDR].Value = value;
			}
		}

		public override System.DateTime Rtc
		{
			get
			{
				return IncotexDateTime.FromIncotexDateTime(
					(IncotexDateTime)_parameters[ParameterNamesMercury203.DateTime].Value);
			}
			set
			{
				//TODO: записать команду в сеть
				throw new NotImplementedException();
			}
		}

        #endregion

        #region Constructors

        public Mercury203(): base()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Инициализирует список свойств для конкретного устройства
        /// </summary>
        protected override void Initialization()
        {
			base.Initialization();

            _parameters.Add(new Parameter(typeof(UInt32))
            {
                Name = ParameterNamesMercury203.GADDR,
                Description = "Групповой адрес счётчика",
                PollingEnabled = true,
                ReadOnly = false,
                ValueConverter = new BigEndianUInt32ValueConverter(),
                Value = (UInt32)0
            });

            _parameters.Add(new Parameter(typeof(IncotexDateTime)) 
            {
                Name = ParameterNamesMercury203.DateTime, 
                Description = "Текущее значение часов счётчика",
                PollingEnabled = true, 
                ReadOnly = false,
                ValueConverter = new IncotexDataTimeTypeConverter(),
                Value = IncotexDateTime.FromDateTime(DateTime.Now)
            });

            _parameters.Add(new Parameter(typeof(float)) 
            {
                Name = ParameterNamesMercury203.PowerLimit,
                Description = "Значение лимита мощности",
                PollingEnabled = true,
                ReadOnly = false,
				ValueConverter = Mpower.GetValueConveter(),
                Value = (float)0
            });

			_parameters.Add(new Parameter(typeof(ushort))
			{
				Name = ParameterNamesMercury203.PowerLimitPerMonth,
				Description = "Значение лимита мощности",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = Menerg.GetValueConveter(),
				Value = (ushort)0
			});

			_parameters.Add(new Parameter(typeof(byte))
			{
				Name = ParameterNamesMercury203.AmountOfActiveTariffs,
				Description = "Количество действующих тарифов",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (byte)0
			});

			_parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203.CounterTarif1,
				Description = "Счётчик тарифа 1",
				PollingEnabled = true,
				ReadOnly = true,
				ValueConverter = null,
				Value = (float)0
			});

			_parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203.CounterTarif2,
				Description = "Счётчик тарифа 2",
				PollingEnabled = true,
				ReadOnly = true,
				ValueConverter = null,
				Value = (float)0
			});

			_parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203.CounterTarif3,
				Description = "Счётчик тарифа 3",
				PollingEnabled = true,
				ReadOnly = true,
				ValueConverter = null,
				Value = (float)0
			});

			_parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203.CounterTarif4,
				Description = "Счётчик тарифа 4",
				PollingEnabled = true,
				ReadOnly = true,
				ValueConverter = null,
				Value = (float)0
			});
        }

        private void GetAnswer(NetworkRequest networkRequest)
        {
            var request = (DataMessage)networkRequest.Request.Request;
            //ищем устройтво
            var device = (Mercury203)_NetworkController.Devices[request.Address];

            switch ((Mercury203CmdCode)request.CmdCode)
            {
				case Mercury203CmdCode.WriteAddress:
					{
						GetAnswerWriteAdderss(networkRequest); break;
					}
				case Mercury203CmdCode.WriteGroupAddress:
					{
						GetAnswerGeneralWriteRequest(networkRequest); break;
					}
				case Mercury203CmdCode.WriteDateTime:
					{
						GetAnswerGeneralWriteRequest(networkRequest); break;
					}
				case Mercury203CmdCode.WriteLimitPower:
					{
						GetAnswerGeneralWriteRequest(networkRequest); break;
					}
				case Mercury203CmdCode.WriteAmountOfActiveTariffs:
					{
						GetAnswerGeneralWriteRequest(networkRequest); break;
					}
                case Mercury203CmdCode.ReadGroupAddress:
                    {
                        GetAnswerReadGroupAddress(networkRequest); break;
                    }
				case Mercury203CmdCode.ReadDateTime:
					{
						GetAnswerReadDateTime(networkRequest); break;
					}
				case Mercury203CmdCode.ReadTariffAccumulators:
					{
						GetAnswerReadTariffAccumulators(networkRequest); break;
					}
				case Mercury203CmdCode.ReadPowerLimit:
					{
						GetAnswerPowerLimit(networkRequest); break;
					}
                default:
                    {
                        throw new NotImplementedException(
                            String.Format("Устройтво Mercury 203 не поддерживает команду с кодом {0}",
                            request.CmdCode));
                    }
            }

        }

		public override void ExecuteCommand(string commandName)
		{
			if (Status == Management.Status.Running)
			{
				switch (commandName)
				{
					case CommandNamesMercury203Virtual.SwitchReleOn:
					case CommandNamesMercury203Virtual.SwitchReleOff:
					default:
						{
							throw new NotSupportedException(String.Format(
								"Попытка выполнить устройством Id={} неизвестную команду cmd={0}",
								Id, commandName));
						}
				}
			}
		}

		public override OperationResult ReadParameter(string parameterName)
		{
			IAsyncRequestResult asyncResult;
 
			switch(parameterName)
			{
				case ParameterNamesMercury203.GADDR:
					{
						asyncResult = ReadGroupAddress(isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.CounterTarif1:
					{
						asyncResult = ReadTariffAccumulators(isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.CounterTarif2:
					{
						asyncResult = ReadTariffAccumulators(isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.CounterTarif3:
					{
						asyncResult = ReadTariffAccumulators(isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.CounterTarif4:
					{
						asyncResult = ReadTariffAccumulators(isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.DateTime:
					{
						asyncResult = ReadDateTime(isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.PowerLimit:
					{
						asyncResult = ReadPowerLimit(isExternalCall: true);
						break;
					}
				default:
					{
						throw new NotSupportedException(String.Format(
							"Чтение праметра {0} не поддерживается", parameterName));
					}
			}
			// Ждём завершения операции
			while (!asyncResult.IsCompleted)
			{
				Thread.Sleep(50);
			}
			// Возвращает результат
			return new OperationResult
			{
				Result = asyncResult.Error,
				Value = Parameters[parameterName].Value
			};
		}

		public override OperationResult WriteParameter(string parameterName, ValueType value)
		{
			IAsyncRequestResult asyncResult;
 
			switch(parameterName)
			{
				case ParameterNamesMercury203.GADDR:
					{
						asyncResult = WriteGroupAddress(addr: (uint)value, isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.DateTime:
					{
						asyncResult = WriteDateTime(value: (DateTime)value, isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.PowerLimit:
					{
						asyncResult = WritePowerLimit(value: (float)value, isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.PowerLimitPerMonth:
					{
						asyncResult = WritePowerLimitPerMonth(value: (float)value, isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.AmountOfActiveTariffs:
					{
						asyncResult = WriteAmountOfActiveTariffs(value: (byte)value, isExternalCall: true);
						break;
					}
				case ParameterNamesMercury203.ActiveTariff:
					{
						asyncResult = WriteActiveTariff(value: (byte)value, isExternalCall: true);
						break;
					}
				default:
					{
						throw new NotSupportedException(String.Format(
							"Запись праметра {0} не поддерживается", parameterName));
					}
			}
			// Ждём завершения операции
			while (!asyncResult.IsCompleted)
			{
				Thread.Sleep(50);
			}
			// Возвращает результат
			return new OperationResult
			{
				Result = asyncResult.Error,
				Value = Parameters[parameterName].Value
			};
		}

        public override void EventHandler_NetworkController_NetwrokRequestCompleted(
            object sender, NetworkRequestCompletedArgs e)
        {
            // Ищем запрос в буфере
            var request = _activeRequests.FirstOrDefault(r => r.Id == e.NetworkRequest.Id);

            if (request == null)
            {
                return;
            }

            switch (request.Status)
            {
                case NetworkRequestStatus.Completed:
                    {
                        if (Errors.CommunicationError)
                        {
                            _Errors.CommunicationError = true;
                            OnErrorOccurred(new DeviceErrorOccuredEventArgs 
							{
 								Id = this.Id,
								Errors = _Errors 
							});
                        }
                        // Разбираем транзакцию
                        GetAnswer(e.NetworkRequest);
                        break;
                    }
                case NetworkRequestStatus.Failed:
                    {
                        if (!Errors.CommunicationError)
                        {
                            _Errors.CommunicationError = true;
                            OnErrorOccurred(new DeviceErrorOccuredEventArgs 
							{ 
								Id = this.Id,
								Errors = _Errors 
							});
                        }
                        // Записываем в журнал причину
                        //TODO: Logger.Error(transaction.ToString());
                        break;
                    }
                default:
                    {
                        // Другие варианты в принципе не возможны...
                        throw new Exception();
                    }
            }

        }

        #endregion

        #region Network API

        /// <summary>
        /// Установка нового сетевого адреса счетчика (CMD=00h)
        /// </summary>
        /// <param name="addr">Текущий сетевой адрес счётчика</param>
        /// <param name="newaddr">Новый сетевой адрес счётчика</param>
        /// <returns></returns>
        public IAsyncRequestResult WriteAddress(UInt32 addr, UInt32 newaddr, bool isExternalCall = true)
        {
            var request = new DataMessage()
            {
                Address = addr,
                CmdCode = Convert.ToByte(Mercury203CmdCode.WriteAddress)
            };
            var transaction = new Transaction(this, TransactionType.UnicastMode, request) 
            { 
                Sender = this 
            };
            var networkRequest = new NetworkRequest(transaction);
 
            if (_NetworkController == null)
            {
                transaction.Start();
                transaction.Abort(new TransactionError 
                { 
                    ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
                    Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
                });
                networkRequest.AsyncRequestResult.SetCompleted();
            }
            else
            {
                _activeRequests.Add(networkRequest);
                _NetworkController.Write(networkRequest, isExternalCall);
            }
            return (IAsyncRequestResult)networkRequest.AsyncRequestResult; 
        }

        /// <summary>
		/// Разбирает ответ от удалённого устройтва по запросу SetNewAddress (CMD=00h)
        /// </summary>
        /// <param name="networkRequest"></param>
        private void GetAnswerWriteAdderss(NetworkRequest networkRequest)
        {
            var request = (DataMessage)networkRequest.Request.Request;

            // Разбираем ответ
            if (networkRequest.Status == NetworkRequestStatus.Completed)
            {
                var requestArray = networkRequest.Request.Request.ToArray();
                var answerArray = networkRequest.CurrentTransaction.Answer.ToArray();
                var command = _activeRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);

                if (command == null)
                {
                    throw new Exception("Не найдена команда с указанной транзакцией");
                }

                if (answerArray.Length != 7)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Неверная длина ответного сообщения";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO: Пишем в лог
                    _activeRequests.Remove(command);
                }

                if (answerArray[4] != request.CmdCode)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO: Пишем в лог
                    _activeRequests.Remove(command);
                }

                // Проверяем новый адрес в запросе и в ответе
                if ((requestArray[6] != answerArray[0]) ||
                    (requestArray[7] != answerArray[1]) ||
                    (requestArray[8] != answerArray[2]) ||
                    (requestArray[9] != answerArray[3]))
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription =
                    //    "Новый адрес счётчика в ответе не соответствует устанавливаемому";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO: Пишем в лог
                    _activeRequests.Remove(command);
                }
                
                //Всё в порядке выполняем изменение сетевого адреса
                var converter = new BigEndianUInt32ValueConverter();
                var adr = (UInt32)converter.FromArray(
                    new Byte[] { answerArray[0], answerArray[1], answerArray[2], answerArray[3] });

                Address = adr;
                //command.Status = Result.OK;
                _activeRequests.Remove(command);
            }
            else
            {
                // Транзакция выполнена с ошибкам
                var command = _activeRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);

                //command.Status = Result.Error;
                //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                //TODO: Пишем в лог
                _activeRequests.Remove(command);
            }
        }

		/// <summary>
		/// Установка нового группового адреса счётчика (CMD=01h)
		/// </summary>
		/// <param name="addr"></param>
		/// <param name="isExternalCall"></param>
		/// <returns></returns>
		public IAsyncRequestResult WriteGroupAddress(uint addr, bool isExternalCall = true)
		{
			var request = new DataMessage(new BigEndianUInt32ValueConverter().ToArray(addr))
			{
				Address = this.Address,
				CmdCode = Convert.ToByte(Mercury203CmdCode.WriteGroupAddress)
			};

			var transaction = new Transaction(this, TransactionType.UnicastMode, request)
			{
				Sender = this
			};

			var networkRequest = new NetworkRequest(transaction);

			if (_NetworkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				_activeRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}
			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		/// <summary>
		/// Разбирает ответ на запрос записи параметра, общий для команд
		/// записи имеющий структуру ADDR-CMD-CRC
		/// </summary>
		/// <param name="networkRequest"></param>
		private void GetAnswerGeneralWriteRequest(NetworkRequest networkRequest)
		{
			// Разбираем ответ
			if (networkRequest.Status == NetworkRequestStatus.Completed)
			{
				var command = _activeRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);

				if (command == null)
				{
					throw new Exception("Не найдена команда с указанной транзакцией");
				}

				if (networkRequest.CurrentTransaction.Answer.ToArray().Length != 7)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Неверная длина ответного сообщения";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				var request = (DataMessage)networkRequest.Request.Request;
				var answer = (DataMessage)networkRequest.CurrentTransaction.Answer;

				// Проверяем новый адрес в запросе и в ответе
				if (request.Address != answer.Address)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Адрес команды в ответе не соответствует адресу в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				if (answer.CmdCode != request.CmdCode)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				//command.Status = Result.OK;
				_activeRequests.Remove(command);
			}
			else
			{
				// Транзакция выполнена с ошибкам
				var command = _activeRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);
				//command.Status = Result.Error;
				//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
				//TODO:
				_activeRequests.Remove(command);
			}
		}

		/// <summary>
		/// Установка времени и даты (CMD=02h)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="isExternalCall"></param>
		/// <returns></returns>
		public IAsyncRequestResult WriteDateTime(DateTime value, bool isExternalCall = true)
		{
			var request = new DataMessage(
				new IncotexDataTimeTypeConverter().ToArray(IncotexDateTime.FromDateTime(value)))
			{
				Address = Address,
				CmdCode = Convert.ToByte(Mercury203CmdCode.WriteDateTime)
			};

			var transaction = new Transaction(this, TransactionType.UnicastMode, request)
			{
				Sender = this
			};

			var networkRequest = new NetworkRequest(transaction);

			if (_NetworkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				_activeRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}
			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		/// <summary>
		/// Широковешательная команда записи времени и даты во все устройтсва
		/// сети с указанным групповым адресом
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="groupAddress">Групповой адрес устройтсв</param>
		/// <param name="networkController"></param>
		/// <param name="isExternalCall"></param>
		/// <returns></returns>
		/// <remarks>Ответ на данный тип запросов не приходит, 
		/// только создаётся временная выдержка</remarks>
		public static IAsyncRequestResult WriteDateTimeInGroupDevices(
			DateTime dateTime, 
			UInt32 groupAddress, 
			INetwrokController networkController, 
			bool isExternalCall = true)
		{
			var request = new DataMessage(
				new IncotexDataTimeTypeConverter().ToArray(IncotexDateTime.FromDateTime(dateTime)))
			{
				Address = groupAddress,
				CmdCode = Convert.ToByte(Mercury203CmdCode.WriteDateTime)
			};

			var transaction = new Transaction(null, TransactionType.BroadcastMode, request)
			{
				Sender = null
			};

			var networkRequest = new NetworkRequest(transaction);

			if (networkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				networkController.Write(networkRequest, isExternalCall);
			}
			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		/// <summary>
		/// Установка лимита мощности (CMD=03h)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="isExternalCall"></param>
		/// <returns></returns>
		public IAsyncRequestResult WritePowerLimit(float value, bool isExternalCall = true)
		{
			var request = new DataMessage(
				Menerg.GetValueConveter().ToArray(value))
			{
				Address = Address,
				CmdCode = Convert.ToByte(Mercury203CmdCode.WriteLimitPower)
			};

			var transaction = new Transaction(this, TransactionType.UnicastMode, request)
			{
				Sender = this
			};

			var networkRequest = new NetworkRequest(transaction);

			if (_NetworkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				_activeRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}
			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		/// <summary>
		/// Установка лимита мощности за месяц (CMD=04h)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="isExternalCall"></param>
		/// <returns></returns>
		public IAsyncRequestResult WritePowerLimitPerMonth(float value, bool isExternalCall = true)
		{
			var request = new DataMessage(
				Mpower.GetValueConveter().ToArray(value))
			{
				Address = Address,
				CmdCode = Convert.ToByte(Mercury203CmdCode.WriteLimitPowerPerMonth)
			};

			var transaction = new Transaction(this, TransactionType.UnicastMode, request)
			{
				Sender = this
			};

			var networkRequest = new NetworkRequest(transaction);

			if (_NetworkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				_activeRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}
			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		/// <summary>
		/// Установка числа действующих тарифов (CMD=0Ah)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="isExternalCall"></param>
		/// <returns></returns>
		public IAsyncRequestResult WriteAmountOfActiveTariffs(byte value, bool isExternalCall = true)
		{
			if ((value < 1) || (value > 4))
			{
				throw new ArgumentOutOfRangeException("Amount",
					"Кол-во активных тарифов должно быть от 1 до 4");
			}

			var request = new DataMessage(
				Mpower.GetValueConveter().ToArray(value))
			{
				Address = Address,
				CmdCode = Convert.ToByte(Mercury203CmdCode.WriteAmountOfActiveTariffs)
			};

			var transaction = new Transaction(this, TransactionType.UnicastMode, request)
			{
				Sender = this
			};

			var networkRequest = new NetworkRequest(transaction);

			if (_NetworkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				_activeRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}

			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		/// <summary>
		/// Установка тарифа (CMD=0Bh)
		/// </summary>
		/// <param name="value"></param>
		/// <param name="isExternalCall"></param>
		/// <returns></returns>
		public IAsyncRequestResult WriteActiveTariff(byte value, bool isExternalCall = true)
		{
			if ((value < 1) || (value > 4))
			{
				throw new ArgumentOutOfRangeException("Amount",
					"Действующий тарифов должн быть от 1 до 4");
			}

			var request = new DataMessage(
				Mpower.GetValueConveter().ToArray(value))
			{
				Address = Address,
				CmdCode = Convert.ToByte(Mercury203CmdCode.WriteActiveTariff)
			};

			var transaction = new Transaction(this, TransactionType.UnicastMode, request)
			{
				Sender = this
			};

			var networkRequest = new NetworkRequest(transaction);

			if (_NetworkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				_activeRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}

			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}
		
        /// <summary>
        /// Чтение группового адреса счетчика (CMD=20h)
        /// </summary>
        [PeriodicReadEnabled]
        public IAsyncRequestResult ReadGroupAddress(bool isExternalCall = true)
        {
            var request = new DataMessage()
            {
                Address = Address,
                CmdCode = Convert.ToByte(Mercury203CmdCode.ReadGroupAddress)
            };
            var transaction = new Transaction(this, TransactionType.UnicastMode, request)
            { 
                Sender = this 
            };

            var networkRequest = new NetworkRequest(transaction);

            if (_NetworkController == null)
            {
                transaction.Start();
                transaction.Abort(new TransactionError
                {
                    ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
                    Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
                });
                networkRequest.AsyncRequestResult.SetCompleted();
            }
            else
            {
                _activeRequests.Add(networkRequest);
                _NetworkController.Write(networkRequest, isExternalCall);
            }
            return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
        }

        /// <summary>
        /// Разбирает ответ по запросу ReadGroupAddress (CMD=20h)
        /// </summary>
        /// <param name="transaction"></param>
        private void GetAnswerReadGroupAddress(NetworkRequest networkRequest)
        {
            // Разбираем ответ
            if (networkRequest.Status == NetworkRequestStatus.Completed)
            {
                var command = _activeRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);

                if (command == null)
                {
                    throw new Exception("Не найдена команда с указанной транзакцией");
                }

                if (networkRequest.CurrentTransaction.Answer.ToArray().Length != 11)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Неверная длина ответного сообщения";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO:
                    _activeRequests.Remove(command);
                }

                var request = (DataMessage)networkRequest.Request.Request;
                var answer = (DataMessage)networkRequest.CurrentTransaction.Answer;

                // Проверяем новый адрес в запросе и в ответе
                if (request.Address != answer.Address)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Адрес команды в ответе не соответствует адресу в запросе";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO:
                    _activeRequests.Remove(command);
                }

                if (answer.CmdCode != request.CmdCode)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO:
                    _activeRequests.Remove(command);
                }

                // Получаем параметр
                // Присваиваем новое значение параметру
                var parameter = _parameters[ParameterNamesMercury203.GADDR];
                parameter.Value = parameter.ValueConverter.FromArray(
                    new byte[] 
                    {
                        answer.Data[0],
                        answer.Data[1],
                        answer.Data[2],
                        answer.Data[3]
                    });

                //command.Status = Result.OK;
                _activeRequests.Remove(command);
            }
            else
            {
                // Транзакция выполнена с ошибкам
                var command = _activeRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);
                //command.Status = Result.Error;
                //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                //TODO:
                _activeRequests.Remove(command);
            }
        }

        /// <summary>
        /// Чтение внутренних часов и календаря счетчика (CMD=21h)
        /// </summary>
        /// <returns></returns>
        [PeriodicReadEnabled]
        public IAsyncRequestResult ReadDateTime(bool isExternalCall = true)
        {
            var request = new DataMessage()
            {
                Address = Address,
                CmdCode = Convert.ToByte(Mercury203CmdCode.ReadDateTime)
            };
            var transaction = new Transaction(this, TransactionType.UnicastMode, request)
            {
                Sender = this
            };

            var networkRequest = new NetworkRequest(transaction);

            if (_NetworkController == null)
            {
                transaction.Start();
                transaction.Abort(new TransactionError
                {
                    ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
                    Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
                });
                networkRequest.AsyncRequestResult.SetCompleted();
            }
            else
            {
                _activeRequests.Add(networkRequest);
                _NetworkController.Write(networkRequest, isExternalCall);
            }
            return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
        }

		/// <summary>
		/// Разбирает ответ по запросу ReadDateTime (CMD=21h)
		/// </summary>
		/// <param name="networkRequest"></param>
		private void GetAnswerReadDateTime(NetworkRequest networkRequest)
		{
			// Разбираем ответ
			if (networkRequest.Status == NetworkRequestStatus.Completed)
			{
				var command = _activeRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);

				if (command == null)
				{
					throw new Exception("Не найдена команда с указанной транзакцией");
				}

				if (networkRequest.CurrentTransaction.Answer.ToArray().Length != 11)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Неверная длина ответного сообщения";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				var request = (DataMessage)networkRequest.Request.Request;
				var answer = (DataMessage)networkRequest.CurrentTransaction.Answer;

				// Проверяем новый адрес в запросе и в ответе
				if (request.Address != answer.Address)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Адрес команды в ответе не соответствует адресу в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				if (answer.CmdCode != request.CmdCode)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				// Получаем параметр
				// Присваиваем новое значение параметру
				var parameter = _parameters[ParameterNamesMercury203.DateTime];
				parameter.Value = parameter.ValueConverter.FromArray(answer.Data.ToArray());

				//command.Status = Result.OK;
				_activeRequests.Remove(command);
			}
			else
			{
				// Транзакция выполнена с ошибкам
				var command = _activeRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);
				//command.Status = Result.Error;
				//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
				//TODO:
				_activeRequests.Remove(command);
			}
		}

		/// <summary>
		/// Чтение лимита мощности (CMD=22h)
		/// </summary>
		/// <returns></returns>
		[PeriodicReadEnabled]
		public IAsyncRequestResult ReadPowerLimit(bool isExternalCall = true)
		{
			var request = new DataMessage()
			{
				Address = Address,
				CmdCode = Convert.ToByte(Mercury203CmdCode.ReadPowerLimit)
			};
			var transaction = new Transaction(this, TransactionType.UnicastMode, request)
			{
				Sender = this
			};

			var networkRequest = new NetworkRequest(transaction);

			if (_NetworkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				_activeRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}
			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		/// <summary>
		/// Разбирает ответ по запросу ReadPowerLimit (CMD=22h)
		/// </summary>
		/// <param name="networkRequest"></param>
		private void GetAnswerPowerLimit(NetworkRequest networkRequest)
		{
			// Разбираем ответ
			if (networkRequest.Status == NetworkRequestStatus.Completed)
			{
				var command = _activeRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);

				if (command == null)
				{
					throw new Exception("Не найдена команда с указанной транзакцией");
				}

				if (networkRequest.CurrentTransaction.Answer.ToArray().Length != 9)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Неверная длина ответного сообщения";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				var request = (DataMessage)networkRequest.Request.Request;
				var answer = (DataMessage)networkRequest.CurrentTransaction.Answer;

				// Проверяем новый адрес в запросе и в ответе
				if (request.Address != answer.Address)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Адрес команды в ответе не соответствует адресу в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				if (answer.CmdCode != request.CmdCode)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				// Получаем параметр
				// Присваиваем новое значение параметру
				var parameter = _parameters[ParameterNamesMercury203.PowerLimit];
				parameter.Value = parameter.ValueConverter.FromArray(answer.Data.ToArray());

				//command.Status = Result.OK;
				_activeRequests.Remove(command);
			}
			else
			{
				// Транзакция выполнена с ошибкам
				var command = _activeRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);
				//command.Status = Result.Error;
				//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
				//TODO:
				_activeRequests.Remove(command);
			}
		}

		/// <summary>
		/// Чтение лимита энергии за месяц
		/// </summary>
		/// <returns></returns>
		[PeriodicReadEnabled]
		public IAsyncRequestResult ReadPowerLimitPerMonth(bool isExternalCall = true)
		{
			var request = new DataMessage()
			{
				Address = Address,
				CmdCode = Convert.ToByte(Mercury203CmdCode.ReadPowerLimitPerMonth)
			};
			var transaction = new Transaction(this, TransactionType.UnicastMode, request)
			{
				Sender = this
			};

			var networkRequest = new NetworkRequest(transaction);

			if (_NetworkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				_activeRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}
			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		/// <summary>
		/// Читает содержимое тарифных аккумуляторов (CMD=27h)
		/// </summary>
		/// <param name="isExternalCall"></param>
		/// <returns></returns>
		public IAsyncRequestResult ReadTariffAccumulators(bool isExternalCall = true)
		{
			var request = new DataMessage()
			{
				Address = Address,
				CmdCode = Convert.ToByte(Mercury203CmdCode.ReadTariffAccumulators)
			};
			var transaction = new Transaction(this, TransactionType.UnicastMode, request)
			{
				Sender = this
			};

			var networkRequest = new NetworkRequest(transaction);

			if (_NetworkController == null)
			{
				transaction.Start();
				transaction.Abort(new TransactionError
				{
					ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
					Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
				});
				networkRequest.AsyncRequestResult.SetCompleted();
			}
			else
			{
				_activeRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}
			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		/// <summary>
		/// Разбирает ответ по запросу ReadTariffAccumulators (CMD=27h)
		/// </summary>
		/// <param name="networkRequest"></param>
		private void GetAnswerReadTariffAccumulators(NetworkRequest networkRequest)
		{
			// Разбираем ответ
			if (networkRequest.Status == NetworkRequestStatus.Completed)
			{
				var command = _activeRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);

				if (command == null)
				{
					throw new Exception("Не найдена команда с указанной транзакцией");
				}

				if (networkRequest.CurrentTransaction.Answer.ToArray().Length != 23)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Неверная длина ответного сообщения";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				var request = (DataMessage)networkRequest.Request.Request;
				var answer = (DataMessage)networkRequest.CurrentTransaction.Answer;

				// Проверяем новый адрес в запросе и в ответе
				if (request.Address != answer.Address)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Адрес команды в ответе не соответствует адресу в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				if (answer.CmdCode != request.CmdCode)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_activeRequests.Remove(command);
				}

				// Получаем параметр
				// Присваиваем новое значение параметру
				TariffCounters cntrs = TariffCounters.FromArray(answer.Data.ToArray());
				
				_parameters[ParameterNamesMercury203.CounterTarif1].Value = cntrs.ValueTotalTarif1;
				_parameters[ParameterNamesMercury203.CounterTarif2].Value = cntrs.ValueTotalTarif2;
				_parameters[ParameterNamesMercury203.CounterTarif3].Value = cntrs.ValueTotalTarif3;
				_parameters[ParameterNamesMercury203.CounterTarif4].Value = cntrs.ValueTotalTarif4;

				//command.Status = Result.OK;
				_activeRequests.Remove(command);
			}
			else
			{
				// Транзакция выполнена с ошибкам
				var command = _activeRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);
				//command.Status = Result.Error;
				//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
				//TODO:
				_activeRequests.Remove(command);
			}
		}

        #endregion
    }
}