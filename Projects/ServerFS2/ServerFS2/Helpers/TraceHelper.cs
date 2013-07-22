using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ServerFS2
{
	public static class TraceHelper
	{
		public static string TraceBytes(IEnumerable<byte> bytes, string description = "")
		{
			var message = (description + " " + String.Join(" ", bytes.Select(p => p.ToString("X2")).ToArray()) + " ");
			Trace.WriteLine(message);
			return message;
		}
	}
}