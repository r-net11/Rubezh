using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;

namespace Infrastructure.Common.Mail
{
	public class MailHelper
	{
		public static void Send(SenderParams senderParams, string to, string body, string subject = "")
		{
			try
			{
				MailMessage message = new MailMessage(senderParams.From, to, subject, body);
				SmtpClient client = new SmtpClient(senderParams.Ip, int.Parse(senderParams.Port));
				client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.Credentials = new System.Net.NetworkCredential(senderParams.UserName, senderParams.Password);
				client.Send(message);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Exception Mail.Send: {0}",
								ex.ToString());
			}
		}

		public static string PresentStates(Email email)
		{
			var presenrationStates = new StringBuilder();
			if (email.States == null)
				email.States = new List<StateType>();
			for (int i = 0; i < email.States.Count; i++)
			{
				if (i > 0)
					presenrationStates.Append(", ");
				presenrationStates.Append(email.States[i].ToDescription());
			}
			return presenrationStates.ToString();
		}
	}
}