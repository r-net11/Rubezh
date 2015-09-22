using System;
using System.Diagnostics;
using System.Text;

namespace RubezhResurs.OSI.Messages.Transaction
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
        /// ���������� ��� ������� ����������
        /// </summary>
        public TransactionType TransactionType
        {
            get { return this._Type; }
        }
        /// <summary>
        /// ��������� ����������
        /// </summary>
        private TransactionStatus _Status;
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
        /// ����� ������ ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        private DateTime _StartTime;
        /// <summary>
        /// ���������� ����� ������ ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        public DateTime StartTime
        {
            get { return _StartTime; }
        }
        /// <summary>
        /// ����� ��������� ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        private DateTime _EndTime;
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
        private IMessage _Request;
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
        private IMessage _Answer;
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
        /// �������� ������� ��������� ���������� ��� ������ ������ Abort()
        /// </summary>
        private String _DescriptionError;
        /// <summary>
        /// ���������� �������� ������� ��������� ���������� ��� ������ ������ Abort()
        /// </summary>
        public String DescriptionError
        {
            get { return _DescriptionError; }
        }        
        #endregion

        #region Constructors
        /// <summary>
        /// �����������
        /// </summary>
        public Transaction() 
        {
            _Answer = null;
            _Request = null;
            _Status = TransactionStatus.NotInitialized;
            _EndTime = DateTime.Now;
            _StartTime = DateTime.Now;
            _Type = TransactionType.Undefined;
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="type">��� modbus ���������</param>
        /// <param name="request">������ �� ������� ����</param>
        public Transaction(TransactionType type, 
            IMessage request)
        {            
            _Type = type;
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
        /// <param name="description">��������� �������� ������ ������� ����������</param>
        public void Abort(String description)
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
                if (description != null)
                {
                    _DescriptionError = description;
                }
                else
                {
                    _DescriptionError = String.Empty;
                }
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
        #endregion

        #region Events
        /// <summary>
        /// ������� ��������� ����� ���������� ����������;
        /// </summary>
        public event EventHandler TransactionWasEnded; 
        #endregion
    }
}
