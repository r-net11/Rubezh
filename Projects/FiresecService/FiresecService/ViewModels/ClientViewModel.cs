using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
	public class ClientViewModel : BaseViewModel
	{
		public FiresecService.Service.FiresecService FiresecService { get; set; }
		public Guid UID { get; set; }
		public string IpAddress { get; set; }
		public int CallbackPort { get; set; }
		public ClientType ClientType { get; set; }
		public DateTime ConnectionDate { get; set; }
		public List<OperationViewModel> Operations { get; set; }

		public ClientViewModel()
		{
			Operations = new List<OperationViewModel>();
		}

		OperationViewModel ClientToServerOperation;
		OperationViewModel ServerToClientOperation;

		OperationViewModel GetOperation(OperationDirection operationDirection)
		{
			switch (operationDirection)
			{
				case OperationDirection.ClientToServer:
					return ClientToServerOperation;

				case OperationDirection.ServerToClient:
					return ServerToClientOperation;
			}
			return null;
		}

		public void BeginAddOperation(OperationDirection operationDirection, string operationName)
		{
			var operation = new OperationViewModel()
			{
				StartDateTime = DateTime.Now,
				OperationName = operationName,
				Direction = operationDirection
			};
			switch(operationDirection)
			{
				case OperationDirection.ClientToServer:
					ClientToServerOperation = operation;
					ClientToServerOperationName = operationName;
					break;

				case OperationDirection.ServerToClient:
					ServerToClientOperation = operation;
					ServerToClientOperationName = operationName;
					break;
			}
		}

		public void EndAddOperation(OperationDirection operationDirection)
		{
			var operation = GetOperation(operationDirection);
			if (operation == null)
			{
				Logger.Error("ClientViewModel.EndAddOperation operation = null");
				return;
			}
			operation.Duration = DateTime.Now - operation.StartDateTime;
			if (operation.OperationName != "Ping")
			{
				Operations.Add(operation);
			}
			operation = null;

			switch (operationDirection)
			{
				case OperationDirection.ClientToServer:
					ClientToServerOperationName = "";
					break;

				case OperationDirection.ServerToClient:
					ServerToClientOperationName = "";
					break;
			}
		}

		string _userName;
		public string UserName
		{
			get { return _userName; }
			set
			{
				_userName = value;
				OnPropertyChanged("UserName");
			}
		}

		string _clientToServerOperationName;
		public string ClientToServerOperationName
		{
			get { return _clientToServerOperationName; }
			set
			{
				_clientToServerOperationName = value;
				OnPropertyChanged("ClientToServerOperationName");
			}
		}

		string _serverToClientOperationName;
		public string ServerToClientOperationName
		{
			get { return _serverToClientOperationName; }
			set
			{
				_serverToClientOperationName = value;
				OnPropertyChanged("ServerToClientOperationName");
			}
		}

		string _state;
		public string State
		{
			get { return _state; }
			set
			{
				_state = value;
				OnPropertyChanged("State");
			}
		}
	}
}