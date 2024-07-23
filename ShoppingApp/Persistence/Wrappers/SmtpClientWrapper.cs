using Domain.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Wrappers
{
	public class SmtpClientWrapper : ISmtpClientWrapper
	{
		public async Task SendMailAsync(SmtpClient client, MailMessage message)
		{
			await client.SendMailAsync(message);
		}
	}
}
