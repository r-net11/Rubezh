using System;

namespace Firesec
{
    public class FiresecOperationResult<T>
    {
        public T Result;
        public bool HasError;
        public Exception Error;
    }
}