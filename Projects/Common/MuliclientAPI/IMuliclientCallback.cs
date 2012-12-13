using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MuliclientAPI
{
	[ServiceContract]
	public interface IMuliclientCallback
	{
		[OperationContract(IsOneWay=true)]
		void Show();

		[OperationContract(IsOneWay = true)]
		void Hide();

		[OperationContract]
        WindowSize GetWindowSize();

        [OperationContract]
        void SetWindowSize(WindowSize windowSize);
	}
}