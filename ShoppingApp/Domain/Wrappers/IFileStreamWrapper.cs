using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Wrappers
{
	public interface IFileStreamWrapper
	{
		Task CopyToAsync(IFormFile file, string imagePath);
	}
}
