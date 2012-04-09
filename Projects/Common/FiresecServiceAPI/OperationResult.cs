using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class OperationResult<T>
    {
        [DataMember]
        public T Result { get; set; }

        [DataMember]
        public bool HasError { get; set; }

        [DataMember]
        public Exception Error { get; set; }
    }
}