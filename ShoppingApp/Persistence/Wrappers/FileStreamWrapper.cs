using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Wrappers
{
	public class FileStreamWrapper : IFileStreamWrapper
	{
		public async Task CopyToAsync(IFormFile file, string imagePath)
		{
			using (var stream = new FileStream(imagePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
		}
	}
}
