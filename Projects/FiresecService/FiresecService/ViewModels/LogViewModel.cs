using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecService.Models
{
	public class LogViewModel
	{
		public LogViewModel(string message, bool isError)
		{
			Message = message;
			IsError = isError;
			DateTime = DateTime.Now;
		}

		public string Message { get; private set; }
		public DateTime DateTime { get; private set; }
		public bool IsError { get; private set; }
	}
}