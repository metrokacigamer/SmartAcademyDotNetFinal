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
		private readonly ISession _session;

		public SessionWrapper(ISession session)
		{
			_session = session;
		}
		public string GetString(string key)
		{
			return _session.GetString(key);
		}

		public void SetString(string key, string value)
		{
			_session.SetString(key, value);
		}
	}
}
