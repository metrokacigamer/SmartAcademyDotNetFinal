using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public class InvalidFileTypeException: Exception
	{
        public InvalidFileTypeException(string extension = "", string message = "Invalid extension"): base(message)
        {
            
        }
    }
}
