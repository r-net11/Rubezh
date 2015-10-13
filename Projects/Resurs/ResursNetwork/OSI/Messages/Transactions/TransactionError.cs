using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursNetwork.OSI.Messages.Transactions
{
    public struct TransactionError
    {
        public TransactionErrorCodes ErrorCode;
        public string Description;
    }

}
