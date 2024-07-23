using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public class ProductExistsException: Exception
	{
		public ProductExistsException(string message = "Product with such name already exists"): base(message)
		{

		}
	}
}
