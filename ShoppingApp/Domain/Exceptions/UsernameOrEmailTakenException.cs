using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public class UsernameOrEmailTakenException: Exception
	{
        public UsernameOrEmailTakenException(string message = "Username or Email already taken"): base(message) { }
    }
}
