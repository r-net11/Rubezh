using System;
using System.Diagnostics;
using System.Text;
using ResursNetwork.OSI.ApplicationLayer.Devices;

namespace ResursNetwork.OSI.Messages.Transactions
{
    /// <summary>
    /// ����� ��� �������� ������ ���������� "������-�����"
    /// </summary>
    public class Transaction
    {
        #region Fields And Properties
        
        /// <summary>
        /// ��� modbus-���������� "������-�����"
        /// </summary>
        private TransactionType _Type;

        /// <summary>
        /// ��������� ����������� ����������
        /// </summary>
        private IDevice _Sender;
        
        /// <summary>
        /// ��������� ����������
        /// </summary>
        private TransactionStatus _Status;
        
        /// <summary>
        /// ����� ������ ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        private DateTime _StartTime;
        
        /// <summary>
        /// ����� ��������� ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        private DateTime _EndTime;
        
        /// <summary>
        /// ������ �� ������� ����
        /// </summary>
        private IMessage _Request;
        
        /// <summary>
        /// ����� �� �������� ����������
        /// </summary>
        private IMessage _Answer;

        private TransactionError _Error = new TransactionError 
            { 
                Description = String.Empty, 
                ErrorCode = TransactionErrorCodes.NoError 
            };

        /// <summary>
        /// ���������� ��� ������� ����������
        /// </summary>
        public TransactionType TransactionType
        {
            get { return this._Type; }
        }

        /// <summary>
        /// ��������� ����������� ����������
        /// </summary>
        public IDevice Sender
        {
            get { return _Sender; }
            set { _Sender = value; }
        }

        /// <summary>
        /// ��������� ����������
        /// </summary>
        public TransactionStatus Status
        {
            get { return _Status; }
        }

        /// <summary>
        /// ���������� ��������� ����������
        /// </summary>
        public Boolean IsRunning
        {
            get
            {
                Boolean result;
                switch (this._Status)
                {
                    case TransactionStatus.Aborted:
                        {
                            result = false; break;
                        }
                    case TransactionStatus.Completed:
                        {
                            result = false; break;
                        }
                    case TransactionStatus.NotInitialized:
                        {
                            result = false; break;
                        }
                    case TransactionStatus.Running:
                        {
                            result = true; break;
                        }
                    default:
                        {
                            throw new NotImplementedException(
                                "������ ��������� ���������� �� �������������� � ������ ������ ��");
                        }
                }
                return result;
            }
        }
                
        /// <summary>
        /// ���������� ����� ������ ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        public DateTime StartTime
        {
            get { return _StartTime; }
        }
                
        /// <summary>
        /// ���������� ����� ��������� ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        public DateTime EndTime
        {
            get { return _EndTime; }
        }
        
        /// <summary>
        /// ������������ ����������, ����
        /// </summary>
        public TimeSpan TotalTime
        {
            get 
            {
                if (IsRunning)
                {
                    return DateTime.Now - _StartTime;
                }
                else
                {
                    return _EndTime - _StartTime;
                }
            }
        }
                
        /// <summary>
        /// ������ �� ������� ����
        /// </summary>
        public IMessage Request
        {
            get { return _Request; }
        }
                
        /// <summary>
        /// ����� �� �������� ����������
        /// </summary>
        public IMessage Answer
        {
            get { return _Answer; }
            set { _Answer = value; }
        }
        
        /// <summary>
        /// ������������� ����������
        /// </summary>
        public Guid Identifier
        {
            get
            {
                return _Request == null ? Guid.Empty : _Request.MessageId;
            }
        }
                
        /// <summary>
        /// ���������� �������� ������� ��������� ���������� ��� ������ ������ Abort()
        /// </summary>
        public TransactionError Error
        {
            get { return _Error; }
        }        
        

        #endregion

        #region Constructors
        /// <summary>
        /// �����������
        /// </summary>
        public Transaction() 
        {
            _Answer = null;
            _Sender = null;
            _Request = null;
            _Status = TransactionStatus.NotInitialized;
            _EndTime = DateTime.Now;
            _StartTime = DateTime.Now;
            _Type = TransactionType.Undefined;
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="sender">���������� �������</param>
        /// <param name="type">��� modbus ���������</param>
        /// <param name="request">������ �� ������� ����</param>
        public Transaction(IDevice sender, TransactionType type, 
            IMessage request)
        {            
            _Type = type;
            _Sender = sender;
            _Answer = null;
            _Request = request;
            _Status = TransactionStatus.NotInitialized;
            _EndTime = new DateTime();
            _StartTime = new DateTime();
        }
        #endregion

        #region Methods
        /// <summary>
        /// �������� ����� ����������
        /// </summary>
        public void Start()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException(String.Format(
                    "Transaction ID: {0} - ������� ��������� ��� ���������� ����������",
                    Identifier));
            }
            else
            {
                _Status = TransactionStatus.Running;
                _StartTime = DateTime.Now;
                OnTransactionWasStarted();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - ������ ����������: {1} ����",
                //    this.Identifier.ToString(), this._TimeOfStart));
            }
            return;
        }
        /// <summary>
        /// ����������� ������� ����������
        /// </summary>
        /// <param name="answer">����� slave-����������</param>
        public void Stop(IMessage answer)
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; ������� ��������� �� ������� ����������", 
                    this.Identifier));
            }
            else
            {

                switch (TransactionType)
                {
                    case TransactionType.UnicastMode:
                        {
                            if (answer != null)
                            {
                                this._Answer = answer;
                            }
                            else
                            {
                                throw new NullReferenceException(
                                    "������� ���������� � null �������� ��������� ��� ���������� " +
                                    "���������� ������������� �������");
                            }
                            break;
                        }
                    case TransactionType.BroadcastMode:
                        {
                            if (answer != null)
                            {
                                throw new InvalidOperationException(
                                    "������� ���������� �������� ��������� ��� ���������� ���������� " +
                                    "������������������ �������");
                            }
                            break;
                        }
                    case TransactionType.Undefined:
                        {
                            this._Answer = answer;
                            break;
                        }
                }

                _EndTime = DateTime.Now;
                _Status = TransactionStatus.Completed;
                
                // ���������� ������� ��������� ����������.
                OnTransactionWasEnded();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - ����� ����������: {1}; ����� ����������: {2}",
                //    this.Identifier, this._TimeOfEnd, this.TimeOfTransaction));
            }
            return;
        }
        /// <summary>
        /// ��������� ������� ����������
        /// </summary>
        /// <param name="error">��������� �������� ������ ������� ����������</param>
        public void Abort(TransactionError error)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; ������� �������� �� ������� ����������",
                    this.Identifier));
            }
            else
            {
                _EndTime = DateTime.Now;
                _Error = error;
                _Status = TransactionStatus.Aborted;
                // ���������� �������
                OnTransactionWasEnded();
            }
            return;
        }
        
        /// <summary>
        /// ����� ����������� ������� ���������� ����������
        /// </summary>
        private void OnTransactionWasEnded()
        {
            EventHandler handler = this.TransactionWasEnded;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;
                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        
        /// <summary>
        /// ���������� ������� ������� ����������
        /// </summary>
        public void OnTransactionWasStarted()
        {
            if (TransactionWasStarted != null)
            {
                TransactionWasStarted(this, new EventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(
                "Transaction: Id={0}; Type={1}; Status={2}; Start={3}; Stop={4}; Error={5}; Request={6}; Answer={7}", 
                Identifier, TransactionType, Status, StartTime, EndTime, Error, 
                Request == null ? String.Empty : Request.ToString(), 
                Answer == null ? String.Empty : Answer.ToString());
            //return base.ToString();
        }
        
        #endregion

        #region Events

        /// <summary>
        /// ������� ��������� ����� ������� ����������
        /// </summary>
        public event EventHandler TransactionWasStarted;
        
        /// <summary>
        /// ������� ��������� ����� ���������� ����������;
        /// </summary>
        public event EventHandler TransactionWasEnded;
        
        #endregion
    }
}
