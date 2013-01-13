using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
	[Serializable]
    public class OperationResult<T>
    {
        public OperationResult()
        {
            HasError = false;
        }

        public OperationResult(string error)
        {
            HasError = true;
            Error = error;
        }

        [DataMember]
        public T Result { get; set; }

        [DataMember]
        public bool HasError { get; set; }

        [DataMember]
        public string Error { get; set; }
    }
}