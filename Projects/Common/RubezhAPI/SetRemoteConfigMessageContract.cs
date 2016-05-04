using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace RubezhAPI
{
	[MessageContract]
	public class SetRemoteConfigMessageContract
	{
		[MessageHeader(MustUnderstand = true)]
		public Guid ClientUid;

		[MessageBodyMember(Order = 1)]
		public Stream Stream;
	}

	[MessageContract]
	public class SetRemoteConfigResult
	{
		[MessageHeader(MustUnderstand = true)]
		public string Error;

		[MessageHeader(MustUnderstand = true)]
		public bool HasError;
	}
}
