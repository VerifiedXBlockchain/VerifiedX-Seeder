using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace RBXOSeed.Utility
{
    public class EmailUtility
    {
		public static async void SendEmail(string ToAddress, string Subject, string Body, string EmailTemp, bool IsHTML = false, string AlertTitle = "")
		{
			try
			{
				if (IsHTML == true)
				{
					if (EmailTemp == "action")
					{
						string path = Path.Combine(Environment.CurrentDirectory, @"EmailTemplates\" + EmailTemp + ".html");
						string bodyTemplate = File.ReadAllText(path);
						bodyTemplate = bodyTemplate.Replace("{ActionBody}", Body);
						Body = bodyTemplate;
					}
					if (EmailTemp == "alert")
					{
						string path = Path.Combine(Environment.CurrentDirectory, @"EmailTemplates\" + EmailTemp + ".html");
						string bodyTemplate = File.ReadAllText(path);
						bodyTemplate = bodyTemplate.Replace("{AlertTitle}", AlertTitle);
						bodyTemplate = bodyTemplate.Replace("{AlertBody}", Body);
						Body = bodyTemplate;
					}
					if (EmailTemp == "login")
					{
						//do nothing. was done in coode
					}
				}


				var fromAddress = new MailAddress("hello@rbx.network", "RBX Network");
				var toAddress = new MailAddress(ToAddress);

				//Email is using amazons ses. We pay per email. Cost is relatively low, but eventually once we build email queue we will want our own maybe.
				var smtp = new SmtpClient
				{
					Host = "[ENTER HOSTNAME]",
					Port = 587,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential("[USERNAME]", "[PASSWORD]"),
					DeliveryMethod = SmtpDeliveryMethod.Network,
					EnableSsl = true
				};
				using (var message = new MailMessage(fromAddress, toAddress)
				{
					IsBodyHtml = true,
					Subject = Subject,
					Body = Body,
				})
				{
					if (!Debugger.IsAttached)
					{
						await smtp.SendMailAsync(message);
					}
				}
			}
			catch (Exception ex)
			{
				using (EventLog eventLog = new EventLog("Application"))
				{
					eventLog.Source = "Application";
					eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Information, 101, 1);
				}
				ex.ToString();
			}
		}

		public static string AddStandardEmailTemplate(string body)
		{
			string emailTemplate = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "EmailTemplates/StandardEmailTemplate.html"));
			emailTemplate = emailTemplate.Replace("[@body]", body);
			return emailTemplate;
		}
	}
}
