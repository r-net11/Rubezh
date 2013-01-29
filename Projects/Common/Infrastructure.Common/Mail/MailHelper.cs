using System;
using System.Diagnostics;
using System.Net.Mail;
using FiresecAPI.Models;
using System.Text;
using FiresecAPI;
using System.Collections.Generic;

namespace Infrastructure.Common.Mail
{
	public class MailHelper
	{
		public static void Send(string to, string body, string subject = "")
		{
			try
			{
				string from = "obychevma@rubezh.ru";
				MailMessage message = new MailMessage(from, to, subject, body);
				SmtpClient client = new SmtpClient("mail.rubezh.ru", 25);
				client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.Credentials = new System.Net.NetworkCredential("obychevma@rubezh.ru", "Aiciir5kee");
				client.Send(message);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Exception Mail.Send: {0}", ex.ToString());
			}
		}

		public static string PresentStates(Email email)
		{
			var presenrationStates = new StringBuilder();
			if (email.SendingStates == null)
				email.SendingStates = new List<StateType>();
			for (int i = 0; i < email.SendingStates.Count; i++)
			{
				if (i > 0)
					presenrationStates.Append(", ");
				presenrationStates.Append(email.SendingStates[i].ToDescription());
			}
			return presenrationStates.ToString();
		}
	}
}