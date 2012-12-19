using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MuliclientAPI
{
    [DataContract]
    public class MulticlientData
    {
		[DataMember]
		public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public int Port { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}