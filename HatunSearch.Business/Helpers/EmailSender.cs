// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.Net.Mail;

namespace HatunSearch.Business.Helpers
{
	public static class EmailSender
	{
		public static void Send(MailAddress to, string subject, string body) => Send(DefaultAddress, to, subject, body);
		private static void Send(MailAddress from, MailAddress to, string subject, string body)
		{
			SmtpClient client = new SmtpClient();
			MailMessage message = new MailMessage(from, to)
			{
				IsBodyHtml = true,
				Subject = subject,
				Body = body
			};
			client.Send(message);
		}

		public static MailAddress DefaultAddress { get; set; }
	}
}