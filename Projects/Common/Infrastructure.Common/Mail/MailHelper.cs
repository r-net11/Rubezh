using System;
using System.Diagnostics;
using System.Net.Mail;

namespace Infrastructure.Common.Mail
{
	public class MailHelper
	{
		public static void Send(string to, string body, string subject = "")
		{
			string from = "obychevma@rubezh.ru";
			MailMessage message = new MailMessage(from, to, subject, body);
			SmtpClient client = new SmtpClient("mail.rubezh.ru", 25);
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.Credentials = new System.Net.NetworkCredential("obychevma@rubezh.ru", "Aiciir5kee");
			try
			{
				client.Send(message);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Exception client.Send(message): {0}",
					  ex.ToString());
			}
		}
	}
}