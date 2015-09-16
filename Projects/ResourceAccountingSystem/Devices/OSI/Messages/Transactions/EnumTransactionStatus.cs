using System;
using System.Collections.Generic;
using System.Text;

namespace RubezhResurs.OSI.Messages.Transaction
{
    /// <summary>
    /// ��������� ����������
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// ����� ���������� ��� ������, � ������� ������ ����������
        /// </summary>
        NotInitialized = 0,
        /// <summary>
        /// ���������� ��������� � �������� ��������� (� �������� ����������)
        /// </summary>
        Running,
        /// <summary>
        /// ���������� ���������
        /// </summary>
        Completed,
        /// <summary>
        /// ���������� ���� �������� ��� ��������.
        /// </summary>
        Aborted
    }
}
