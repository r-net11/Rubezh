using System;
using System.Diagnostics;
using System.Net.Mail;

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
				Trace.WriteLine("Exception Mail.Send: {0}",
								ex.ToString());
			}
		}
	}
}