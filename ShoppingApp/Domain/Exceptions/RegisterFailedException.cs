using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public class RegisterFailedException: Exception
	{
        public IdentityResult Result { get; init; }
        public RegisterFailedException(IdentityResult result, string message = "Error Occured") : base(message)
        {
            Result = result;
        }
    }
}
