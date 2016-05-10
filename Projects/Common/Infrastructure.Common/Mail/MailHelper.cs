using Common;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.Models;
using Infrastructure.Common.Windows;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Infrastructure.Common.Mail
{
	public class MailHelper
	{
		public static void Send(EmailSettings senderParams, string to, string body, string subject = "")
		{
			try
			{
				if (IsValidEmailSettings(senderParams))
				{
					MailMessage message = new MailMessage(senderParams.UserName, to, subject, body);
					SmtpClient client = new SmtpClient(senderParams.Ip, int.Parse(senderParams.Port));
					client.DeliveryMethod = SmtpDeliveryMethod.Network;
					client.Credentials = new System.Net.NetworkCredential(senderParams.UserName, senderParams.Password);
					client.Send(message);
				}
				else
				{
					MessageBoxService.ShowError(Resources.Language.MailHelper.MailHelper.Smtp_Error);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MailHelper.Send");
			}
		}

		public static string PresentStates(Email email)
		{
			var presenrationStates = new StringBuilder();
			if (email.States == null)
				email.States = new List<XStateClass>();
			for (int i = 0; i < email.States.Count; i++)
			{
				if (i > 0)
					presenrationStates.Append(", ");
				presenrationStates.Append(email.States[i].ToDescription());
			}
			return presenrationStates.ToString();
		}

		private static bool IsValidEmailSettings(EmailSettings emailSettings)
		{
			return !(emailSettings == null ||
				emailSettings.Ip == null ||
				emailSettings.Password == null ||
				emailSettings.Port == null ||
				emailSettings.UserName == null);
		}
	}
}