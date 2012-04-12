using System;

namespace Firesec
{
    public class FiresecOperationResult<T>
    {
        public T Result;
        public bool HasError;
        public Exception Error;

        public string ErrorString
        {
            get
            {
                if (Error != null)
                    return Error.Message.ToString();
                return null;
            }
        }
    }
}