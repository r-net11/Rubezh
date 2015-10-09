using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.Messages.Transactions;

namespace ResursNetwork.OSI.ApplicationLayer.Devices
{
    public enum Result
    {
        /// <summary>
        /// Находится в режиме выполения
        /// </summary>
        Active = 0,
        OK,
        Error
    }
    public class DeviceCommand
    {
        #region Fields And Properties

        private Guid _Id;
        private NetworkRequest _NetworkRequest;
        private Result _Status;
        private string _ErrorDescription = String.Empty;

        public Guid Id
        {
            get { return _Id; }
        }

        public NetworkRequest NetworkRequest
        {
            get { return _NetworkRequest; }
            set { _NetworkRequest = value; }
        }

        /// <summary>
        /// Состояние выполнения транзакции
        /// </summary>
        public Result Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        /// <summary>
        /// Описание ошибки при выполении транзакции
        /// </summary>
        public string ErrorDescription
        {
            get { return _ErrorDescription; }
            set { _ErrorDescription = value; }
        }

        #endregion

        #region Constructors

        public DeviceCommand()
        {
            _Status = Result.Active;
            _Id = Guid.NewGuid();
        }

        #endregion

        #region Methods
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Статус операции=");
            sb.Append(Status.ToString());
            sb.Append("; ");
            sb.Append("Ошибка=");
            sb.Append(ErrorDescription);
            sb.Append("; ");
            sb.Append("Запрос = ");
            sb.Append(NetworkRequest.ToString());

            return sb.ToString();
        }
        #endregion
    }
}
