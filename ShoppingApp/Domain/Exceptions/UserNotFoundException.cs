﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public class UserNotFoundException: Exception
	{
		public UserNotFoundException(string message = "No such user"): base(message) { }
	}
}
