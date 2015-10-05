using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.Messages.Transaction;

namespace ResursNetwork.Devices
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
        private Guid _Id;

        public Guid Id
        {
            get { return _Id; }
        }

        private Transaction _Transaction;

        public Transaction Transaction
        {
            get { return _Transaction; }
            set { _Transaction = value; }
        }

        private Result _Status;
        /// <summary>
        /// Состояние выполнения транзакции
        /// </summary>
        public Result Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        private string _ErrorDescription = String.Empty;
        /// <summary>
        /// Описание ошибки при выполении транзакции
        /// </summary>
        public string ErrorDescription
        {
            get { return _ErrorDescription; }
            set { _ErrorDescription = value; }
        }

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
            sb.Append("Транзакция = ");
            sb.Append(Transaction.ToString());

            return sb.ToString();
        }
        #endregion
    }
}
