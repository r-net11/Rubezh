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
        private List<NetworkRequest> _ActiveRequests = new List<NetworkRequest>();

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

		public override System.DateTime Rtc
		{
			get
			{
				return IncotexDateTime.FromIncotexDateTime(
					(IncotexDateTime)_Parameters[ParameterNamesMercury203.DateTime].Value);
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

            _Parameters.Add(new Parameter(typeof(UInt32))
            {
                Name = ParameterNamesMercury203.GADDR,
                Description = "Групповой адрес счётчика",
                PollingEnabled = true,
                ReadOnly = false,
                ValueConverter = new BigEndianUInt32ValueConverter(),
                Value = (UInt32)0
            });

            _Parameters.Add(new Parameter(typeof(IncotexDateTime)) 
            {
                Name = ParameterNamesMercury203.DateTime, 
                Description = "Текущее значение часов счётчика",
                PollingEnabled = true, 
                ReadOnly = false,
                ValueConverter = new IncotexDataTimeTypeConverter(),
                Value = IncotexDateTime.FromDateTime(DateTime.Now)
            });

            _Parameters.Add(new Parameter(typeof(float)) 
            {
                Name = ParameterNamesMercury203.PowerLimit,
                Description = "Значение лимита мощности",
                PollingEnabled = true,
                ReadOnly = false,
				ValueConverter = Mpower.GetValueConveter(),
                Value = (float)0
            });

			_Parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203.CounterTarif1,
				Description = "Счётчик тарифа 1",
				PollingEnabled = true,
				ReadOnly = true,
				ValueConverter = null,
				Value = (float)0
			});

			_Parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203.CounterTarif2,
				Description = "Счётчик тарифа 2",
				PollingEnabled = true,
				ReadOnly = true,
				ValueConverter = null,
				Value = (float)0
			});

			_Parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203.CounterTarif3,
				Description = "Счётчик тарифа 3",
				PollingEnabled = true,
				ReadOnly = true,
				ValueConverter = null,
				Value = (float)0
			});

			_Parameters.Add(new Parameter(typeof(float))
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
				case Mercury203CmdCode.WriteDateTime:
					{
						GetAnswerWriteDateTime(networkRequest); break;
					}
                case Mercury203CmdCode.SetNetworkAddress:
                    {
                        GetAnswerWriteNetwokAdderss(networkRequest); break;
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
				case ParameterNamesMercury203.CounterTarif1:
					{
						asyncResult = ReadTariffAccumulators(isExternalCall: true);
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
				case ParameterNamesMercury203.CounterTarif2:
					{
						asyncResult = ReadTariffAccumulators(isExternalCall: true);
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
				case ParameterNamesMercury203.CounterTarif3:
					{
						asyncResult = ReadTariffAccumulators(isExternalCall: true);
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
				case ParameterNamesMercury203.CounterTarif4:
					{
						asyncResult = ReadTariffAccumulators(isExternalCall: true);
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
				case ParameterNamesMercury203.DateTime:
					{
						asyncResult = ReadDateTime(isExternalCall: true);
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
				case ParameterNamesMercury203.PowerLimit:
					{
						asyncResult = ReadPowerLimit(isExternalCall: true);
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
				default:
					{
						throw new NotSupportedException(String.Format(
							"Чтение праметра {0} не поддерживается", parameterName));
					}
			}
		}

		public override OperationResult WriteParameter(string parameterName, ValueType value)
		{
			IAsyncRequestResult asyncResult;
 
			switch(parameterName)
			{
				case ParameterNamesMercury203.DateTime:
					{
						asyncResult = WriteDateTime(value: (DateTime)value, isExternalCall: true);
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

						break;
					}
				default:
					{
						throw new NotSupportedException(String.Format(
							"Запись праметра {0} не поддерживается", parameterName));
					}
			}
		}

        public override void EventHandler_NetworkController_NetwrokRequestCompleted(
            object sender, NetworkRequestCompletedArgs e)
        {
            // Ищем запрос в буфере
            var request = _ActiveRequests.FirstOrDefault(r => r.Id == e.NetworkRequest.Id);

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
        /// Установка нового сетевого адреса счетчика (CMD=)
        /// </summary>
        /// <param name="addr">Текущий сетевой адрес счётчика</param>
        /// <param name="newaddr">Новый сетевой адрес счётчика</param>
        /// <returns></returns>
        public IAsyncRequestResult WriteNewAddress(UInt32 addr, UInt32 newaddr, bool isExternalCall = true)
        {
            var request = new DataMessage()
            {
                Address = addr,
                CmdCode = Convert.ToByte(Mercury203CmdCode.SetNetworkAddress)
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
                networkRequest.AsyncRequestResult.SetCompleted(new Transaction[] { transaction });
            }
            else
            {
                _ActiveRequests.Add(networkRequest);
                _NetworkController.Write(networkRequest, isExternalCall);
            }
            return (IAsyncRequestResult)networkRequest.AsyncRequestResult; 
        }

        /// <summary>
        /// Разбирает ответ от удалённого устройтва по запросу SetNewAddress
        /// </summary>
        /// <param name="networkRequest"></param>
        private void GetAnswerWriteNetwokAdderss(NetworkRequest networkRequest)
        {
            var request = (DataMessage)networkRequest.Request.Request;

            // Разбираем ответ
            if (networkRequest.Status == NetworkRequestStatus.Completed)
            {
                var requestArray = networkRequest.Request.Request.ToArray();
                var answerArray = networkRequest.CurrentTransaction.Answer.ToArray();
                var command = _ActiveRequests.FirstOrDefault(
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
                    _ActiveRequests.Remove(command);
                }

                if (answerArray[4] != request.CmdCode)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO: Пишем в лог
                    _ActiveRequests.Remove(command);
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
                    _ActiveRequests.Remove(command);
                }
                
                //Всё в порядке выполняем изменение сетевого адреса
                var converter = new BigEndianUInt32ValueConverter();
                var adr = (UInt32)converter.FromArray(
                    new Byte[] { answerArray[0], answerArray[1], answerArray[2], answerArray[3] });

                Address = adr;
                //command.Status = Result.OK;
                _ActiveRequests.Remove(command);
            }
            else
            {
                // Транзакция выполнена с ошибкам
                var command = _ActiveRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);

                //command.Status = Result.Error;
                //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                //TODO: Пишем в лог
                _ActiveRequests.Remove(command);
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
				networkRequest.AsyncRequestResult.SetCompleted(new Transaction[] { transaction });
			}
			else
			{
				_ActiveRequests.Add(networkRequest);
				_NetworkController.Write(networkRequest, isExternalCall);
			}
			return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
		}

		private void GetAnswerWriteDateTime(NetworkRequest networkRequest)
		{
			// Разбираем ответ
			if (networkRequest.Status == NetworkRequestStatus.Completed)
			{
				var command = _ActiveRequests.FirstOrDefault(
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
					_ActiveRequests.Remove(command);
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
					_ActiveRequests.Remove(command);
				}

				if (answer.CmdCode != request.CmdCode)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_ActiveRequests.Remove(command);
				}

				//command.Status = Result.OK;
				_ActiveRequests.Remove(command);
			}
			else
			{
				// Транзакция выполнена с ошибкам
				var command = _ActiveRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);
				//command.Status = Result.Error;
				//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
				//TODO:
				_ActiveRequests.Remove(command);
			}
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
                networkRequest.AsyncRequestResult.SetCompleted(new Transaction[] { transaction });
            }
            else
            {
                _ActiveRequests.Add(networkRequest);
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
                var command = _ActiveRequests.FirstOrDefault(
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
                    _ActiveRequests.Remove(command);
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
                    _ActiveRequests.Remove(command);
                }

                if (answer.CmdCode != request.CmdCode)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO:
                    _ActiveRequests.Remove(command);
                }

                // Получаем параметр
                // Присваиваем новое значение параметру
                var parameter = _Parameters[ParameterNamesMercury203.GADDR];
                parameter.Value = parameter.ValueConverter.FromArray(
                    new byte[] 
                    {
                        answer.Data[0],
                        answer.Data[1],
                        answer.Data[2],
                        answer.Data[3]
                    });

                //command.Status = Result.OK;
                _ActiveRequests.Remove(command);
            }
            else
            {
                // Транзакция выполнена с ошибкам
                var command = _ActiveRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);
                //command.Status = Result.Error;
                //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                //TODO:
                _ActiveRequests.Remove(command);
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
                networkRequest.AsyncRequestResult.SetCompleted(new Transaction[] { transaction });
            }
            else
            {
                _ActiveRequests.Add(networkRequest);
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
				var command = _ActiveRequests.FirstOrDefault(
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
					_ActiveRequests.Remove(command);
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
					_ActiveRequests.Remove(command);
				}

				if (answer.CmdCode != request.CmdCode)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_ActiveRequests.Remove(command);
				}

				// Получаем параметр
				// Присваиваем новое значение параметру
				var parameter = _Parameters[ParameterNamesMercury203.DateTime];
				parameter.Value = parameter.ValueConverter.FromArray(answer.Data.ToArray());

				//command.Status = Result.OK;
				_ActiveRequests.Remove(command);
			}
			else
			{
				// Транзакция выполнена с ошибкам
				var command = _ActiveRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);
				//command.Status = Result.Error;
				//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
				//TODO:
				_ActiveRequests.Remove(command);
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
				networkRequest.AsyncRequestResult.SetCompleted(new Transaction[] { transaction });
			}
			else
			{
				_ActiveRequests.Add(networkRequest);
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
				var command = _ActiveRequests.FirstOrDefault(
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
					_ActiveRequests.Remove(command);
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
					_ActiveRequests.Remove(command);
				}

				if (answer.CmdCode != request.CmdCode)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_ActiveRequests.Remove(command);
				}

				// Получаем параметр
				// Присваиваем новое значение параметру
				var parameter = _Parameters[ParameterNamesMercury203.PowerLimit];
				parameter.Value = parameter.ValueConverter.FromArray(answer.Data.ToArray());

				//command.Status = Result.OK;
				_ActiveRequests.Remove(command);
			}
			else
			{
				// Транзакция выполнена с ошибкам
				var command = _ActiveRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);
				//command.Status = Result.Error;
				//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
				//TODO:
				_ActiveRequests.Remove(command);
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
				networkRequest.AsyncRequestResult.SetCompleted(new Transaction[] { transaction });
			}
			else
			{
				_ActiveRequests.Add(networkRequest);
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
				networkRequest.AsyncRequestResult.SetCompleted(new Transaction[] { transaction });
			}
			else
			{
				_ActiveRequests.Add(networkRequest);
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
				var command = _ActiveRequests.FirstOrDefault(
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
					_ActiveRequests.Remove(command);
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
					_ActiveRequests.Remove(command);
				}

				if (answer.CmdCode != request.CmdCode)
				{
					//command.Status = Result.Error;
					//command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
					//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
					//TODO:
					_ActiveRequests.Remove(command);
				}

				// Получаем параметр
				// Присваиваем новое значение параметру
				TariffCounters cntrs = TariffCounters.FromArray(answer.Data.ToArray());
				
				_Parameters[ParameterNamesMercury203.CounterTarif1].Value = cntrs.ValueTotalTarif1;
				_Parameters[ParameterNamesMercury203.CounterTarif2].Value = cntrs.ValueTotalTarif2;
				_Parameters[ParameterNamesMercury203.CounterTarif3].Value = cntrs.ValueTotalTarif3;
				_Parameters[ParameterNamesMercury203.CounterTarif4].Value = cntrs.ValueTotalTarif4;

				//command.Status = Result.OK;
				_ActiveRequests.Remove(command);
			}
			else
			{
				// Транзакция выполнена с ошибкам
				var command = _ActiveRequests.FirstOrDefault(
					p => p.Id == networkRequest.Id);
				//command.Status = Result.Error;
				//OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
				//TODO:
				_ActiveRequests.Remove(command);
			}
		}

        #endregion
    }
}
