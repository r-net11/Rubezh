﻿
using System.Runtime.Serialization;
namespace FiresecClient.Models
{
    [DataContract]
    public class Property
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
