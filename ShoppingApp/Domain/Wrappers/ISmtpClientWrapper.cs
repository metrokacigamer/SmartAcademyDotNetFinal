﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Wrappers
{
	public interface ISmtpClientWrapper
	{
		Task SendMailAsync(SmtpClient client, MailMessage message);
	}
}
