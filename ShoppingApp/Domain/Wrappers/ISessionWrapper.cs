using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Wrappers
{
	public interface ISessionWrapper
	{
		string GetString(string key);
		void SetString(string key, string value);
	}
}
