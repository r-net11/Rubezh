using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.Messages.Transactions;

namespace ResursNetwork.OSI.ApplicationLayer
{
	public class NetworkRequestCompletedArgs : EventArgs
	{
		/// <summary>
		/// Завершённый сетевой запрос
		/// </summary>
		NetworkRequest _networkRequest;

		public NetworkRequest NetworkRequest
		{
			get { return _networkRequest; }
			set { _networkRequest = value; }
		}
	}
}
