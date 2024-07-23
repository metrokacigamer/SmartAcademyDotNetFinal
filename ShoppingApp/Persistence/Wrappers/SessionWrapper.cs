using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Wrappers
{
	public class SessionWrapper : ISessionWrapper
	{
		private readonly IHttpContextAccessor _context;

		public SessionWrapper(IHttpContextAccessor session)
		{
			_context = session;
		}

		public string GetString(string key)
		{
			return _context.HttpContext.Session.GetString(key);
		}

		public void SetString(string key, string value)
		{
			_context.HttpContext.Session.SetString(key, value);
		}
	}
}
